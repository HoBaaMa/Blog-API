using Blog_API.Data;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Repositories.Implementations
{
    public class LikeRepository : ILikeRepository
    {
        private readonly BlogDbContext _context;
        public LikeRepository(BlogDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Like like)
        {
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Like>> GetLikesByBlogPostIdAsync(Guid blogPostId)
        {
            return await _context.Likes
                .Where(l => l.BlogPostId == blogPostId)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Like>> GetLikesByCommentIdAsync(Guid commentId)
        {
            return await _context.Likes
                .Where(l => l.CommentId == commentId)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<Like?> GetByUserAndTargetAsync(string userId, Guid? blogPostId, Guid? commentId)
        {
            return await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId 
            && l.BlogPostId == blogPostId 
            && l.CommentId == commentId);
        }

        public async Task RemoveAsync(Like like)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }

    }
}
