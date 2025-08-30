using AutoMapper;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Blog_API.Services.Interface;
using Blog_API.Utilities;

namespace Blog_API.Services.Implementation
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogPostService> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="BlogPostService"/> with required dependencies.
        /// </summary>
        /// <remarks>
        /// All parameters are required; passing null for any dependency will cause an <see cref="ArgumentNullException"/> to be thrown.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when any required dependency is null.</exception>
        public BlogPostService(IMapper mapper, IBlogPostRepository blogPostRepository, ITagRepository tagRepository, ILogger<BlogPostService> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new blog post for the specified user, validating and persisting any provided images and tags.
        /// </summary>
        /// <param name="createBlogPostDTO">Data for the blog post to create (title, content, optional image URLs and tag names).</param>
        /// <param name="userId">Identifier of the user who will own the created blog post.</param>
        /// <returns>The created blog post, including its persisted tags and image URLs, mapped to <see cref="BlogPostDTO"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when one or more provided image URLs are invalid.</exception>
        public async Task<BlogPostDTO> CreateBlogPostAsync(CreateBlogPostDTO createBlogPostDTO, string userId)
        {
            _logger.LogInformation("Creating blog post for user {UserId} with title '{Title}'", userId, createBlogPostDTO.Title);

            try
            {
                var blogPost = _mapper.Map<BlogPost>(createBlogPostDTO);
                blogPost.UserId = userId;

                // Handle images validation and processing
                if (createBlogPostDTO.ImageUrls?.Any() == true)
                {
                    _logger.LogDebug("Processing {ImageCount} images for blog post", createBlogPostDTO.ImageUrls.Count);

                    // Validate image URLs
                    var (isValid, invalidUrls) = ImageUrlValidator.ValidateImageUrls(createBlogPostDTO.ImageUrls);
                    if (!isValid)
                    {
                        _logger.LogWarning("Invalid image URLs provided: {InvalidUrls}", string.Join(", ", invalidUrls));
                        throw new ArgumentException($"Invalid image URLs: {string.Join(", ", invalidUrls)}");
                    }

                    // Remove duplicates and empty URLs
                    var validImageUrls = createBlogPostDTO.ImageUrls
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Distinct()
                        .ToList();

                    blogPost.ImageUrls = validImageUrls;
                    _logger.LogDebug("Added {ValidImageCount} valid images to blog post", validImageUrls.Count);
                }

                // Handle tags - create or find existing tags and associate them with the blog post
                if (createBlogPostDTO.Tags?.Any() == true)
                {
                    _logger.LogDebug("Processing {TagCount} tags for blog post", createBlogPostDTO.Tags.Count);

                    var tagsToAssociate = new List<Tag>();
                    foreach (var tagName in createBlogPostDTO.Tags)
                    {
                        var tag = await _tagRepository.GetTagByNameAsync(tagName);

                        if (tag == null)
                        {
                            _logger.LogDebug("Creating new tag: {TagName}", tagName);
                            tag = new Tag { Id = Guid.NewGuid(), Name = tagName };
                            await _tagRepository.AddAsync(tag);
                        }
                        else
                        {
                            _logger.LogDebug("Using existing tag: {TagName}", tagName);
                        }
                        tagsToAssociate.Add(tag);
                    }
                    blogPost.Tags = tagsToAssociate;
                }

                await _blogPostRepository.AddAsync(blogPost);
                _logger.LogInformation("Blog post created successfully with ID {BlogPostId} for user {UserId}", blogPost.Id, userId);

                // Get the created blog post with all related data
                var createdBlogPost = await _blogPostRepository.GetByIdAsync(blogPost.Id);
                return _mapper.Map<BlogPostDTO>(createdBlogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog post for user {UserId} with title '{Title}'", userId, createBlogPostDTO.Title);
                throw;
            }
        }

        /// <summary>
        /// Deletes the blog post with the specified ID if it exists and is owned by the given user.
        /// </summary>
        /// <param name="id">The identifier of the blog post to delete.</param>
        /// <param name="currentUserId">The ID of the user attempting the deletion; must match the post's owner.</param>
        /// <exception cref="KeyNotFoundException">Thrown when no blog post exists with the provided <paramref name="id"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the <paramref name="currentUserId"/> does not match the post's owner.</exception>
        public async Task DeleteBlogPostAsync(Guid id, string currentUserId)
        {
            _logger.LogInformation("Attempting to delete blog post {BlogPostId} by user {UserId}", id, currentUserId);

            try
            {
                var blogPost = await _blogPostRepository.GetByIdAsync(id);

                if (blogPost == null)
                {
                    _logger.LogWarning("Blog post {BlogPostId} not found for deletion", id);
                    throw new KeyNotFoundException($"Blog Post ID: {id} not found.");
                }

                if (blogPost.UserId != currentUserId)
                {
                    _logger.LogWarning("Unauthorized deletion attempt for blog post {BlogPostId} by user {UserId}. Post owner: {OwnerUserId}",
                        id, currentUserId, blogPost.UserId);
                    throw new UnauthorizedAccessException();
                }

                await _blogPostRepository.DeleteAsync(blogPost);
                _logger.LogInformation("Blog post {BlogPostId} deleted successfully by user {UserId}", id, currentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog post {BlogPostId} by user {UserId}", id, currentUserId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all blog posts with optional filtering and sorting.
        /// </summary>
        /// <param name="filterOn">The field to filter on (e.g., "Title", "Author"). If null, no filtering is applied.</param>
        /// <param name="filterQuery">The filter value to match against the <paramref name="filterOn"/> field. Ignored when <paramref name="filterOn"/> is null.</param>
        /// <param name="sortBy">The field to sort by (e.g., "CreatedAt", "Title"). If null, the repository's default sort is used.</param>
        /// <param name="isAscending">Whether results should be sorted in ascending order. Defaults to true.</param>
        /// <returns>A read-only collection of BlogPostDTO objects matching the specified filter and sort criteria.</returns>
        public async Task<IReadOnlyCollection<BlogPostDTO>> GetAllBlogPostsAsync(string? filterOn, string? filterQuery, string? sortBy, bool? isAscending = true)
        {
            _logger.LogInformation("Retrieving all blog posts");

            try
            {
                var blogPosts = await _blogPostRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending);
                _logger.LogInformation("Retrieved {BlogPostCount} blog posts", blogPosts.Count);


                return _mapper.Map<IReadOnlyCollection<BlogPostDTO>>(blogPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all blog posts");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously retrieves a blog post by its identifier and returns it as a <see cref="BlogPostDTO"/>.
        /// </summary>
        /// <param name="id">The identifier of the blog post to retrieve.</param>
        /// <returns>The mapped <see cref="BlogPostDTO"/> for the requested blog post.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when a blog post with the specified <paramref name="id"/> does not exist.</exception>
        public async Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving blog post {BlogPostId}", id);

            try
            {
                var blogPost = await _blogPostRepository.GetByIdAsync(id);

                if (blogPost == null)
                {
                    _logger.LogWarning("Blog post {BlogPostId} not found", id);
                    throw new KeyNotFoundException($"Blog post ID: {id} not found.");
                }

                _logger.LogInformation("Blog post {BlogPostId} retrieved successfully", id);
                return _mapper.Map<BlogPostDTO>(blogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog post {BlogPostId}", id);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a paginated list of blog posts filtered by the specified category.
        /// </summary>
        /// <param name="blogCategory">The category to filter blog posts by.</param>
        /// <param name="paginationRequest">Pagination parameters (PageNumber and PageSize) used to page the results.</param>
        /// <returns>
        /// A <see cref="PagedResult{BlogPostDTO}"/> containing the page of blog posts, the total matching count, and the provided page metadata.
        /// </returns>
        public async Task<PagedResult<BlogPostDTO>> GetBlogPostsByCategoryAsync(BlogCategory blogCategory, PaginationRequest paginationRequest)
        {
            _logger.LogInformation("Retrieving paginated blog posts for category {BlogCategory}, page {PageNumber}, size {PageSize}",
                blogCategory, paginationRequest.PageNumber, paginationRequest.PageSize);

            try
            {
                var (blogPosts, totalCount) = await _blogPostRepository.GetBlogPostsByCategoryAsync(blogCategory, paginationRequest);

                _logger.LogInformation("Retrieved {BlogPostCount} blog posts out of {TotalCount} for category {BlogCategory}",
                    blogPosts.Count, totalCount, blogCategory);

                var blogPostDTOs = _mapper.Map<IReadOnlyCollection<BlogPostDTO>>(blogPosts);

                return new PagedResult<BlogPostDTO>
                {
                    Data = blogPostDTOs,
                    TotalCount = totalCount,
                    PageNumber = paginationRequest.PageNumber,
                    PageSize = paginationRequest.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated blog posts for category {BlogCategory}", blogCategory);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing blog post owned by the specified user with values from the provided DTO and returns the updated post.
        /// </summary>
        /// <remarks>
        /// The method updates title, content, category, images, and tags. Image URLs are validated and deduplicated; tags that do not exist will be created and associated with the post.
        /// </remarks>
        /// <param name="id">The identifier of the blog post to update.</param>
        /// <param name="blogPostDTO">Data transfer object containing the updated blog post fields (title, content, category, image URLs, and tag names).</param>
        /// <param name="currentUserId">Identifier of the user attempting the update; must match the post owner.</param>
        /// <returns>The updated <see cref="BlogPostDTO"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a blog post with the specified <paramref name="id"/> does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the <paramref name="currentUserId"/> is not the owner of the blog post.</exception>
        /// <exception cref="ArgumentException">Thrown if one or more provided image URLs are invalid.</exception>
        public async Task<BlogPostDTO> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO, string currentUserId)
        {
            _logger.LogInformation("Updating blog post {BlogPostId} by user {UserId}", id, currentUserId);

            try
            {
                // 1. Get existing blog post
                var existingBlogPost = await _blogPostRepository.GetByIdAsync(id);

                if (existingBlogPost == null)
                {
                    _logger.LogWarning("Blog post {BlogPostId} not found for update", id);
                    throw new KeyNotFoundException($"Blog post ID: {id} not found.");
                }

                // 2. Authorization check
                if (existingBlogPost.UserId != currentUserId)
                {
                    _logger.LogWarning("Unauthorized update attempt for blog post {BlogPostId} by user {UserId}. Post owner: {OwnerUserId}",
                        id, currentUserId, existingBlogPost.UserId);
                    throw new UnauthorizedAccessException();
                }

                _logger.LogDebug("Updating blog post {BlogPostId} properties", id);

                // 3. Update basic properties manually to avoid AutoMapper conflicts
                existingBlogPost.Title = blogPostDTO.Title;
                existingBlogPost.Content = blogPostDTO.Content;
                existingBlogPost.BlogCategory = blogPostDTO.BlogCategory;
                existingBlogPost.UpdatedAt = DateTime.UtcNow;

                // 4. Handle images
                existingBlogPost.ImageUrls.Clear(); // Clear existing images
                _logger.LogDebug("Cleared existing images for blog post {BlogPostId}", id);

                if (blogPostDTO.ImageUrls?.Any() == true)
                {
                    _logger.LogDebug("Processing {ImageCount} images for blog post update", blogPostDTO.ImageUrls.Count);

                    // Validate image URLs
                    var (isValid, invalidUrls) = ImageUrlValidator.ValidateImageUrls(blogPostDTO.ImageUrls);
                    if (!isValid)
                    {
                        _logger.LogWarning("Invalid image URLs provided during update: {InvalidUrls}", string.Join(", ", invalidUrls));
                        throw new ArgumentException($"Invalid image URLs: {string.Join(", ", invalidUrls)}");
                    }

                    // Remove duplicates and empty URLs
                    var validImageUrls = blogPostDTO.ImageUrls
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Distinct()
                        .ToList();

                    foreach (var imageUrl in validImageUrls)
                    {
                        existingBlogPost.ImageUrls.Add(imageUrl);
                    }

                    _logger.LogDebug("Updated blog post {BlogPostId} with {ValidImageCount} valid images", id, validImageUrls.Count);
                }

                // 5. Handle tags
                existingBlogPost.Tags.Clear(); // Clear existing tags
                _logger.LogDebug("Cleared existing tags for blog post {BlogPostId}", id);

                if (blogPostDTO.Tags?.Any() == true)
                {
                    _logger.LogDebug("Processing {TagCount} tags for blog post update", blogPostDTO.Tags.Count);

                    var tagsToAssociate = new List<Tag>();
                    foreach (var tagName in blogPostDTO.Tags)
                    {
                        var tag = await _tagRepository.GetTagByNameAsync(tagName);
                        if (tag == null)
                        {
                            _logger.LogDebug("Creating new tag during update: {TagName}", tagName);
                            tag = new Tag { Id = Guid.NewGuid(), Name = tagName };
                            await _tagRepository.AddAsync(tag);
                        }
                        tagsToAssociate.Add(tag);
                    }
                    existingBlogPost.Tags = tagsToAssociate;
                }

                // 6. Save via repository
                await _blogPostRepository.UpdateAsync(existingBlogPost);
                _logger.LogInformation("Blog post {BlogPostId} updated successfully by user {UserId}", id, currentUserId);

                // 7. Get updated blog post and return DTO
                var updatedBlogPost = await _blogPostRepository.GetByIdAsync(id);
                return _mapper.Map<BlogPostDTO>(updatedBlogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog post {BlogPostId} by user {UserId}", id, currentUserId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the image URLs associated with the specified blog post.
        /// </summary>
        /// <param name="blogPostId">The ID of the blog post whose image URLs are requested.</param>
        /// <returns>A read-only collection of image URL strings associated with the blog post.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when a blog post with <paramref name="blogPostId"/> does not exist.</exception>
        public async Task<IReadOnlyCollection<string>> GetBlogPostImagesAsync(Guid blogPostId)
        {
            _logger.LogInformation("Retrieving images for blog post {BlogPostId}", blogPostId);

            try
            {
                var blogPost = await _blogPostRepository.GetByIdAsync(blogPostId);

                if (blogPost == null)
                {
                    _logger.LogWarning("Blog post {BlogPostId} not found when retrieving images", blogPostId);
                    throw new KeyNotFoundException($"Blog post ID: {blogPostId} not found.");
                }

                _logger.LogInformation("Retrieved {ImageCount} images for blog post {BlogPostId}",
                    blogPost.ImageUrls.Count, blogPostId);

                return blogPost.ImageUrls.ToList().AsReadOnly();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving images for blog post {BlogPostId}", blogPostId);
                throw;
            }
        }
    }
}
