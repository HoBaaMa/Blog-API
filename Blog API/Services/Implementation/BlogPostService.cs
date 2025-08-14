using Blog_API.Data;
using Blog_API.DTOs;
using Blog_API.Models;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Services.Implementation
{
    public class BlogPostService : IBlogPostService
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public BlogPostService(BlogDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<BlogPostDTO?> CreateBlogPostAsync(CreateBlogPostDTO blogPostDTO, string userId)
        {
            if (blogPostDTO == null)
            {
                throw new ArgumentNullException(nameof(blogPostDTO), "Blog post cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(blogPostDTO.Title))
            {
                throw new ArgumentNullException("Blog title cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(blogPostDTO.Content))
            {
                throw new ArgumentNullException("Blog content cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be empty.");
            }
            var blogPost = new BlogPost
            {
                Title = blogPostDTO.Title,
                Content = blogPostDTO.Content,
                UserId = userId
            };

            await _context.blogPosts.AddAsync(blogPost);
            await _context.SaveChangesAsync();

            var responseDTO = new BlogPostDTO
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                UserId = userId,
                CreatedAt = blogPost.CreatedAt,
                UserName = blogPost.User?.UserName ?? "Unknown User"
            };

            return responseDTO;
        }

        public async Task DeleteBlogPostAsync(Guid id)
        {
            var blogPost = await _context.blogPosts
                .FirstOrDefaultAsync(bp => bp.Id == id);

            if (blogPost == null)
            {
                throw new ArgumentNullException("Blog post not found");
            }
            _context.blogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BlogPostDTO>> GetAllBlogPostsAsync()
        {
            var blogPosts = await _context.blogPosts
                .Include(bp => bp.User)
                .Include(bp => bp.Comments)
                .ToListAsync();
            if (blogPosts.Count == 0)
            {
                throw new ArgumentNullException("Blog posts is empty.");
            }
            var blogPostsDTOs = blogPosts.Select(bp => new BlogPostDTO
            {
                Id = bp.Id,
                Title = bp.Title,
                Content = bp.Content,
                CreatedAt = bp.CreatedAt,
                UserId = bp.UserId,
                UserName = bp.User?.UserName ?? "Unknown User",
            }).ToList();

            return blogPostsDTOs;
        }

        public async Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id)
        {
            var blogPost = await _context.blogPosts
                .Include(bp => bp.User)
                .Include(bp => bp.Comments)
                .FirstOrDefaultAsync(bp => bp.Id == id);

            if (blogPost == null)
            {
                throw new ArgumentNullException("Blog post not found");
            }

            var blogPostDTO = new BlogPostDTO
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                CreatedAt = blogPost.CreatedAt,
                UserId = blogPost.UserId,
                UserName = blogPost.User?.UserName ?? "Unknown User",
            };

            return blogPostDTO;
        }

        public async Task<BlogPost?> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO)
        {
            var blogPost = await _context.blogPosts
                .FirstOrDefaultAsync(bp => bp.Id == id);
            if (blogPost == null)
            {
                throw new ArgumentNullException("Blog post not found");
            }
            if (blogPostDTO == null)
            {
                throw new ArgumentNullException(nameof(blogPostDTO), "Blog post cannot be null.");
            }
            if (blogPostDTO.Title == null)
            {
                throw new ArgumentNullException("Blog title cannot be empty.");
            }
            if (blogPostDTO.Content == null)
            {
                throw new ArgumentNullException("Blog content cannot be empty.");
            }
            //var user = await _userManager.FindByIdAsync(blogPost.UserId);
            blogPost.Title = blogPostDTO.Title;
            blogPost.Content = blogPostDTO.Content;
            blogPost.UpdatedAt = DateTime.UtcNow;

            _context.Entry(blogPost).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return blogPost;
        }
    }
}
