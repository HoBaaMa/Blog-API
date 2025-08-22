using AutoMapper.QueryableExtensions;
using Blog_API.Data;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Repositories.Implementations
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogDbContext _context;
        public BlogPostRepository(BlogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(BlogPost blogPost)
        {
            await _context.BlogPosts.AddAsync(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<BlogPost>> GetAllAsync()
        {
            return await _context.BlogPosts              
                .AsNoTracking()
                .Include(bp => bp.User)
                .Include(bp => bp.Likes)
                .Include(c => c.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.User)
                .Include(c => c.Comments
                    .Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                    .ThenInclude(u => u.User)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id) =>
            await _context.BlogPosts
                .Include(bp => bp.User)
                .Include(bp => bp.Likes)
                .Include(c => c.Comments.Where(c=> c.ParentCommentId == null))
                    .ThenInclude(c => c.User)
                .Include(bp => bp.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(bp => bp.Id == id);

        public async Task UpdateAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
        }
    }
}
