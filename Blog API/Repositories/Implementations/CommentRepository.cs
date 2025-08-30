using Blog_API.Data;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogDbContext _context;
        public CommentRepository(BlogDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Comment>> GetAllForBlogPostAsync(Guid blogPostId)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.BlogPostId == blogPostId && c.ParentCommentId == null)
                .Include(c => c.User)
                .Include(c => c.Likes)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Likes)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Likes)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Likes)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)  
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> IsParentExistsAsync(Guid parentCommentId, Guid blogPostId)
        {
            return await _context.Comments.AnyAsync(c => c.ParentCommentId == parentCommentId && c.BlogPostId == blogPostId);
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }
    }
}
