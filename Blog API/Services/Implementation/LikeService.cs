using Blog_API.Data;
using Blog_API.DTOs;
using Blog_API.Models;
using Blog_API.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Services.Implementation
{
    public class LikeService : ILikeService
    {
        private readonly BlogDbContext _context;
        public LikeService(BlogDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ToggleLikeAsync(CreateLikeDTO likeDTO, string userId)
        {
            if (!likeDTO.CommentId.HasValue && !likeDTO.BlogPostId.HasValue)
            {
                throw new ArgumentException("You must specify a post or comment.");
            }
            if (likeDTO.CommentId.HasValue && likeDTO.BlogPostId.HasValue)
            {
                throw new ArgumentException("You cannot like a comment and a blog post at the same time.");
            }
            // Check if the comment id or blog post id is found or not and 


            var existingLike = await _context.likes
                .FirstOrDefaultAsync(l =>
                    l.UserId == userId && (l.CommentId == likeDTO.CommentId || l.BlogPostId == likeDTO.BlogPostId));
            if (existingLike != null)
            {
                _context.likes.Remove(existingLike);
                await _context.SaveChangesAsync();
                return false;
            }

            var like = new Like
            {
                UserId = userId,
                BlogPostId = likeDTO.BlogPostId,
                CommentId = likeDTO.CommentId
            };

            await _context.likes.AddAsync(like);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
