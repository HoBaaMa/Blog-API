using AutoMapper;
using AutoMapper.QueryableExtensions;
using Blog_API.Data;
using Blog_API.DTOs;
using Blog_API.Exceptions;
using Blog_API.Models;
using Blog_API.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Services.Implementation
{
    public class BlogPostService : IBlogPostService
    {
        private readonly BlogDbContext _context;
        private readonly IMapper _mapper;
        public BlogPostService(BlogDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<BlogPostDTO> CreateBlogPostAsync(CreateBlogPostDTO createBlogPostDTO, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var blogPost = _mapper.Map<BlogPost>(createBlogPostDTO);
                blogPost.UserId = userId;

                // Handle tags
                if (createBlogPostDTO.Tags.Any())
                {
                    blogPost.Tags = await ProcessTagsAsync(createBlogPostDTO.Tags);
                }

                _context.BlogPosts.Add(blogPost);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Return the created blog post with all related data
                var createdBlogPost = await _context.BlogPosts
                    .Include(bp => bp.User)
                    .Include(bp => bp.Tags)
                    .Include(bp => bp.Likes)
                    .FirstAsync(bp => bp.Id == blogPost.Id);

                return _mapper.Map<BlogPostDTO>(createdBlogPost);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Deletes a blog post by its identifier if the specified user is the owner.
        /// </summary>
        /// <param name="id">The identifier of the blog post to delete.</param>
        /// <param name="currentUserId">The identifier of the user attempting the deletion; must match the post's owner.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when the specified user does not own the blog post.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when no blog post exists with the given <paramref name="id"/>.</exception>
        /// <exception cref="DatabaseOperationException">Thrown when the database delete operation fails (wraps <see cref="DbUpdateException"/>).</exception>
        public async Task DeleteBlogPostAsync(Guid id, string currentUserId)
        {
            var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
            if (blogPost?.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException();
            }
            if (blogPost == null)
            {
                throw new KeyNotFoundException($"Blog Post ID: {id} not found.");
            }

            _context.BlogPosts.Remove(blogPost);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException("Failed to delete blog post", ex);
            }
        }

        public async Task<IReadOnlyCollection<BlogPostDTO>> GetAllBlogPostsAsync()
        {
            //var blogPosts = await _context.BlogPosts
            //.AsNoTracking()
            //.Include(bp => bp.User)
            //.Include(bp => bp.Likes)
            //.Include(bp => bp.Comments
            //    .Where(c => c.ParentCommentId == null)) // Only top-level comments
            //    .ThenInclude(c => c.User)
            //.Include(bp => bp.Comments
            //    .Where(c => c.ParentCommentId == null))
            //    .ThenInclude(c => c.Replies)
            //    .ThenInclude(r => r.User)
            //.ToListAsync();

            var blogPosts = await _context.BlogPosts
                .AsNoTracking()
                .Include(c => c.Comments
                    .Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                    .ThenInclude(u => u.User)
                .ProjectTo<BlogPostDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();


            if (blogPosts.Count == 0)
            {
                throw new KeyNotFoundException("Blog posts is   empty.");
            }

            return blogPosts;
            //return _mapper.Map<IReadOnlyCollection<BlogPostDTO>>(blogPosts);
        }

        public async Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id)
        {
            var blogPost = await _context.BlogPosts
                .AsNoTracking()
                .Include(bp => bp.Comments
                    .Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                    .ThenInclude(r => r.User)
                .ProjectTo<BlogPostDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(bp => bp.Id == id);

            if (blogPost == null)
            {
                throw new KeyNotFoundException($"Blog post ID: {id} not found.");
            }

            return blogPost;
        }
        /// <summary>
        /// Updates an existing blog post with values from the provided DTO and returns the updated post as a DTO.
        /// </summary>
        /// <param name="id">The identifier of the blog post to update.</param>
        /// <param name="blogPostDTO">The DTO containing updated blog post data (title, content, tags, etc.).</param>
        /// <returns>The updated blog post mapped to <see cref="BlogPostDTO"/>.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the requester does not own the blog post.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when no blog post with the specified <paramref name="id"/> exists.</exception>
        /// <exception cref="DatabaseOperationException">Thrown when saving changes to the database fails.</exception>
        public async Task<BlogPostDTO> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO)
        {
            var blogPost = await _context.BlogPosts
                .Include(u => u.User)
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(bp => bp.Id == id);
                
            if (blogPost?.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException();
            }
            
            if (blogPost == null)
            {
                throw new KeyNotFoundException($"Blog post ID: {id} not found.");
            }

            if (blogPostDTO.Tags.Any())
            {
                var updatedTags = await ProcessTagsAsync(blogPostDTO.Tags);
                blogPost.Tags = updatedTags;
            }

            _mapper.Map(blogPostDTO, blogPost);
            blogPost.UpdatedAt = DateTime.Now;

            
            _context.Entry(blogPost).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException("Failed to delete blog post", ex);
            }
            return _mapper.Map<BlogPostDTO>(blogPost);
        }
        // PUT THIS METHOD HERE - as a private helper method
        private async Task<ICollection<Tag>> ProcessTagsAsync(ICollection<string> tagNames)
        {
            // Clean and normalize tag names
            var normalizedTagNames = tagNames
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToUpperInvariant())
                .Distinct()
                .ToList();

            if (!normalizedTagNames.Any())
                return new List<Tag>();

            var processedTags = new List<Tag>();

            // Check each tag individually
            foreach (var tagName in normalizedTagNames)
            {
                var existingTag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name.ToUpper() == tagName);

                if (existingTag != null)
                {
                    processedTags.Add(existingTag);
                }
                else
                {
                    // Create new tag
                    var newTag = new Tag { Id = Guid.NewGuid(), Name = tagName };
                    _context.Tags.Add(newTag);
                    processedTags.Add(newTag);
                }
            }

            return processedTags;
        }

    }
}
