using Blog_API.Data;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blog_API.Repositories.Implementations
{
    public class LikeRepository : ILikeRepository
    {
        private readonly BlogDbContext _context;
        private readonly ILogger<LikeRepository> _logger;
        
        public LikeRepository(BlogDbContext context, ILogger<LikeRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAsync(Like like)
        {
            _logger.LogDebug("Adding new like to database for user {UserId}", like.UserId);
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Like added successfully with ID {LikeId}", like.Id);
        }

        public async Task<IReadOnlyCollection<Like>> GetLikesByBlogPostIdAsync(Guid blogPostId)
        {
            _logger.LogDebug("Retrieving likes from database for blog post {BlogPostId}", blogPostId);
            return await _context.Likes
                .Where(l => l.BlogPostId == blogPostId)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Like>> GetLikesByCommentIdAsync(Guid commentId)
        {
            _logger.LogDebug("Retrieving likes from database for comment {CommentId}", commentId);
            return await _context.Likes
                .Where(l => l.CommentId == commentId)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<Like?> GetByUserAndTargetAsync(string userId, Guid? blogPostId, Guid? commentId)
        {
            var target = blogPostId.HasValue ? $"blog post {blogPostId}" : $"comment {commentId}";
            _logger.LogDebug("Checking existing like for user {UserId} on {Target}", userId, target);
            
            return await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId 
                && l.BlogPostId == blogPostId 
                && l.CommentId == commentId);
        }

        public async Task RemoveAsync(Like like)
        {
            _logger.LogDebug("Removing like with ID {LikeId} from database", like.Id);
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Like with ID {LikeId} removed successfully", like.Id);
        }
    }
}
