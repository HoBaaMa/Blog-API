using AutoMapper;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Blog_API.Services.Interface;

namespace Blog_API.Services.Implementation
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogPostService> _logger;
        
        public BlogPostService(IMapper mapper, IBlogPostRepository blogPostRepository, ITagRepository tagRepository, ILogger<BlogPostService> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BlogPostDTO> CreateBlogPostAsync(CreateBlogPostDTO createBlogPostDTO, string userId)
        {
            _logger.LogInformation("Creating blog post for user {UserId} with title '{Title}'", userId, createBlogPostDTO.Title);
            
            try
            {
                var blogPost = _mapper.Map<BlogPost>(createBlogPostDTO);
                blogPost.UserId = userId;

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

        public async Task<IReadOnlyCollection<BlogPostDTO>> GetAllBlogPostsAsync()
        {
            _logger.LogInformation("Retrieving all blog posts");
            
            try
            {
                var blogPosts = await _blogPostRepository.GetAllAsync();
                _logger.LogInformation("Retrieved {BlogPostCount} blog posts", blogPosts.Count);


                return _mapper.Map<IReadOnlyCollection<BlogPostDTO>>(blogPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all blog posts");
                throw;
            }
        }

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

        public async Task<IReadOnlyCollection<BlogPostDTO>> GetBlogPostsByCategoryAsync(BlogCategory blogCategory)
        {
            _logger.LogInformation("Retrieving blog posts for category {BlogCategory}", blogCategory);
            
            try
            {
                var blogPosts = await _blogPostRepository.GetBlogPostsByCategoryAsync(blogCategory);
                _logger.LogInformation("Retrieved {BlogPostCount} blog posts for category {BlogCategory}", blogPosts.Count, blogCategory);

                return _mapper.Map<IReadOnlyCollection<BlogPostDTO>>(blogPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog posts for category {BlogCategory}", blogCategory);
                throw;
            }
        }

        public async Task<PagedResult<BlogPostDTO>> GetBlogPostsByCategoryPagedAsync(BlogCategory blogCategory, PaginationRequest paginationRequest)
        {
            _logger.LogInformation("Retrieving paginated blog posts for category {BlogCategory}, page {PageNumber}, size {PageSize}", 
                blogCategory, paginationRequest.PageNumber, paginationRequest.PageSize);
            
            try
            {
                var (blogPosts, totalCount) = await _blogPostRepository.GetBlogPostsByCategoryPagedAsync(blogCategory, paginationRequest);
                
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

                // 4. Handle tags
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

                // 5. Save via repository
                await _blogPostRepository.UpdateAsync(existingBlogPost);
                _logger.LogInformation("Blog post {BlogPostId} updated successfully by user {UserId}", id, currentUserId);

                // 6. Get updated blog post and return DTO
                var updatedBlogPost = await _blogPostRepository.GetByIdAsync(id);
                return _mapper.Map<BlogPostDTO>(updatedBlogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog post {BlogPostId} by user {UserId}", id, currentUserId);
                throw;
            }
        }
    }
}
