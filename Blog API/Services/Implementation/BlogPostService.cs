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
        public BlogPostService(BlogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<BlogPostDTO> CreateBlogPostAsync(CreateBlogPostDTO blogPostDTO, string userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstAsync(u => u.Id == userId);

            var blogPost = new BlogPost
            {
                Title = blogPostDTO.Title,
                Content = blogPostDTO.Content,
                User = user,
                UserId = userId
            };

            await _context.blogPosts.AddAsync(blogPost);
            await _context.SaveChangesAsync();

            return new BlogPostDTO
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                UserId = userId,
                CreatedAt = blogPost.CreatedAt,
                UserName = user.UserName!
            };
        }

        public async Task DeleteBlogPostAsync(Guid id, string currentUserId)
        {
            var blogPost = await _context.blogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
            if (blogPost?.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException();
            }
            if (blogPost == null)
            {
                throw new KeyNotFoundException($"Blog Post ID: {id} not found.");
            }

            _context.blogPosts.Remove(blogPost);

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
            var blogPosts = await _context.blogPosts
                .Include(bp => bp.User)
                .Include(bp => bp.Comments)
                .Include(bp => bp.Likes)
                .AsNoTracking()
                .ToListAsync();

            if (blogPosts.Count == 0)
            {
                throw new KeyNotFoundException("Blog posts is empty.");
            }
            var blogPostsDTOs = blogPosts.Select(bp => new BlogPostDTO
            {
                Id = bp.Id,
                Title = bp.Title,
                Content = bp.Content,
                CreatedAt = bp.CreatedAt,
                UserId = bp.UserId,
                UserName = bp.User?.UserName!,
                LikeCount = bp.Likes.Count
            }).ToList();

            return blogPostsDTOs;
        }

        public async Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id)
        {
            var blogPost = await _context.blogPosts
                .Include(bp => bp.User)
                .Include(bp => bp.Comments)
                .Include(bp => bp.Likes)
                .FirstOrDefaultAsync(bp => bp.Id == id);

            if (blogPost == null)
            {
                throw new KeyNotFoundException($"Blog post ID: {id} not found.");
            }

            return new BlogPostDTO
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                CreatedAt = blogPost.CreatedAt,
                UserId = blogPost.UserId,
                UserName = blogPost.User!.UserName!,
                LikeCount = blogPost.Likes.Count
            };
        }

        public async Task<BlogPostDTO> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO)
        {
            var blogPost = await _context.blogPosts
                .FirstOrDefaultAsync(bp => bp.Id == id);

            if (blogPost == null)
            {
                throw new KeyNotFoundException($"Blog post ID: {id} not found.");
            }

            blogPost.Title = blogPostDTO.Title;
            blogPost.Content = blogPostDTO.Content;
            blogPost.UpdatedAt = DateTime.UtcNow;

            
            _context.Entry(blogPost).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException("Failed to delete blog post", ex);
            }

        }
    }
}
