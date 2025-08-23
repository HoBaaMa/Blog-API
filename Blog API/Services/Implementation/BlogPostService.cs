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
        
        public BlogPostService(IMapper mapper, IBlogPostRepository blogPostRepository, ITagRepository tagRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
        }

        public async Task<BlogPostDTO> CreateBlogPostAsync(CreateBlogPostDTO createBlogPostDTO, string userId)
        {
            var blogPost = _mapper.Map<BlogPost>(createBlogPostDTO);
            blogPost.UserId = userId;

            // Handle tags - create or find existing tags and associate them with the blog 

            if (createBlogPostDTO.Tags?.Any() == true)
            {
                var tagsToAssociate = new List<Tag>();
                foreach (var tagName in createBlogPostDTO.Tags)
                {
                    var tag = await _tagRepository.GetTagByNameAsync(tagName);
                    
                    if (tag == null)
                    {
                        tag = new Tag { Id = Guid.NewGuid(), Name = tagName };
                        await _tagRepository.AddAsync(tag);
                    }
                    tagsToAssociate.Add(tag);
                }
                blogPost.Tags = tagsToAssociate;
            }

            //if (createBlogPostDTO.Tags?.Any() == true)
            //{
            //    blogPost.Tags = createBlogPostDTO.Tags
            //        .Where(t => !string.IsNullOrWhiteSpace(t))
            //        .Select(t => new Tag { Name = t.Trim() })
            //        .ToList();
            //}

            await _blogPostRepository.AddAsync(blogPost);

            // Get the created blog post with all related data
            var createdBlogPost = await _blogPostRepository.GetByIdAsync(blogPost.Id);
            return _mapper.Map<BlogPostDTO>(createdBlogPost);
        }

        public async Task DeleteBlogPostAsync(Guid id, string currentUserId)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            
            if (blogPost == null)
            {
                throw new KeyNotFoundException($"Blog Post ID: {id} not found.");
            }
            
            if (blogPost.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException();
            }

            await _blogPostRepository.DeleteAsync(blogPost);
        }

        public async Task<IReadOnlyCollection<BlogPostDTO>> GetAllBlogPostsAsync()
        {
            var blogPosts = await _blogPostRepository.GetAllAsync();

            if (blogPosts.Count == 0)
            {
                throw new KeyNotFoundException("Blog posts is empty.");
            }

            return _mapper.Map<IReadOnlyCollection<BlogPostDTO>>(blogPosts);
        }

        public async Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);

            if (blogPost == null)
            {
                throw new KeyNotFoundException($"Blog post ID: {id} not found.");
            }

            return _mapper.Map<BlogPostDTO>(blogPost);
        }

        public async Task<IReadOnlyCollection<BlogPostDTO>> GetBlogPostsByCategoryAsync(BlogCategory blogCategory)
        {
            var blogPosts = await _blogPostRepository.GetBlogPostsByCategoryAsync(blogCategory);

            return _mapper.Map<IReadOnlyCollection<BlogPostDTO>>(blogPosts);
        }

        public async Task<PagedResult<BlogPostDTO>> GetBlogPostsByCategoryPagedAsync(BlogCategory blogCategory, PaginationRequest paginationRequest)
        {
            var (blogPosts, totalCount) = await _blogPostRepository.GetBlogPostsByCategoryPagedAsync(blogCategory, paginationRequest);

            var blogPostDTOs = _mapper.Map<IReadOnlyCollection<BlogPostDTO>>(blogPosts);

            return new PagedResult<BlogPostDTO>
            {
                Data = blogPostDTOs,
                TotalCount = totalCount,
                PageNumber = paginationRequest.PageNumber,
                PageSize = paginationRequest.PageSize
            };
        }

        public async Task<BlogPostDTO> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO, string currentUserId)
        {
            // 1. Get existing blog post
            var existingBlogPost = await _blogPostRepository.GetByIdAsync(id);

            if (existingBlogPost == null)
                throw new KeyNotFoundException($"Blog post ID: {id} not found.");

            // 2. Authorization check
            if (existingBlogPost.UserId != currentUserId)
                throw new UnauthorizedAccessException();

            // 3. Update basic properties manually to avoid AutoMapper conflicts
            existingBlogPost.Title = blogPostDTO.Title;
            existingBlogPost.Content = blogPostDTO.Content;
            existingBlogPost.BlogCategory = blogPostDTO.BlogCategory;
            existingBlogPost.UpdatedAt = DateTime.UtcNow;

            // 4. Handle tags
            existingBlogPost.Tags.Clear(); // Clear existing tags

            if (blogPostDTO.Tags?.Any() == true)
            {
                var tagsToAssociate = new List<Tag>();
                foreach (var tagName in blogPostDTO.Tags)
                {
                    var tag = await _tagRepository.GetTagByNameAsync(tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Id = Guid.NewGuid(), Name = tagName };
                        await _tagRepository.AddAsync(tag);
                    }
                    tagsToAssociate.Add(tag);
                }
                existingBlogPost.Tags = tagsToAssociate;
            }

            // 5. Save via repository
            await _blogPostRepository.UpdateAsync(existingBlogPost);

            // 6. Get updated blog post and return DTO
            var updatedBlogPost = await _blogPostRepository.GetByIdAsync(id);
            return _mapper.Map<BlogPostDTO>(updatedBlogPost);
        }
    }
}
