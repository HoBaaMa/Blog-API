using Blog_API.Data;
using Blog_API.DTOs;
using Blog_API.Models;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog_API.Exceptions;

namespace Blog_API.Services.Implementation
{
    public class CommentService : ICommentService
    {
        private readonly BlogDbContext _context;
        public CommentService(BlogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<CommentDTO> CreateCommentAsync(CreateCommentDTO commentDTO, string userId)
        {
            var comment = new Comment
            {
                Content = commentDTO.Content,
                BlogPostId = commentDTO.BlogPostId,
                ParentCommentId = commentDTO.ParentCommentId,
                UserId = userId
            };

            /*
             * Brainstorming:
             * ParentCommentId = null => means it's top-level comment
             * ParenctCommentId != null => means it's a reply to the parent comment with that ID
             * EF Core will handle the relationship automatically and add the reply comment to the Replies collection of the parent comment.
             */
            if (commentDTO.ParentCommentId.HasValue)
            {
                bool parentExists = await _context.comments.AnyAsync(c => c.Id == commentDTO.ParentCommentId.Value && c.BlogPostId == commentDTO.BlogPostId);
                if (!parentExists)
                {
                    throw new KeyNotFoundException($"Parent comment with ID: {commentDTO.ParentCommentId.Value} not found.");
                }
            }
            await _context.comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            var responseDTO = new CommentDTO
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserId = comment.UserId,
                UserName = comment.User.UserName!,
                LikeCount = comment.Likes.Count,
                ParentCommentId = comment.ParentCommentId
            };
            return responseDTO;
        }

        public async Task DeleteCommentAsync(Guid commentId, string currentUserId)
        {
            var isOwner = await _context.comments.AnyAsync(c => c.Id == commentId && c.UserId == currentUserId);
            if (!isOwner)
            {
                bool exists = await _context.comments.AnyAsync(c => c.Id == commentId);
                throw exists ? new UnauthorizedAccessException("Access denied") : new KeyNotFoundException($"Comment ID: {commentId} not found.");
            }
            _context.comments.Remove(new Comment { Id = commentId});
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException("Failed to delete comment", ex);
            }
            
        }

        public async Task<IReadOnlyCollection<CommentDTO>> GetAllCommentsForBlogPostAsync(Guid blogPostId)
        {
            // blogPostId is not found exception

            if (!await _context.blogPosts.AnyAsync(bp => bp.Id == blogPostId))
            {
                throw new KeyNotFoundException($"Blog Post with ID {blogPostId} not found.");
            }

            // Use AsNoTracking for read-only queries to improve performance
            var comments = await _context.comments
                .Where(c => c.BlogPostId == blogPostId)
                .AsNoTracking()
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.UserName!,
                    // Count on collection navigation translates; no null-propagation needed
                    LikeCount = c.Likes.Count(),
                    ParentCommentId = c.ParentCommentId
                })
                .ToListAsync();

            if (comments.Count() == 0)
            {
                throw new KeyNotFoundException($"No comments found for Blog Post ID {blogPostId}.");
            }

            return comments;
        }

        public async Task<CommentDTO?> GetCommentByIdAsync(Guid commentId)
        {
            var comment = await _context.comments
                .Include(c => c.User)
                .Include(l => l.Likes)
                .Include(r => r.Replies)
                    .ThenInclude(r => r.User)
                .Include(r => r.Replies)
                    .ThenInclude(u => u.Likes)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
            }

            var responseDTO = new CommentDTO
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserId = comment.UserId,
                UserName = comment.User.UserName!,
                LikeCount = comment.Likes.Count,
                ParentCommentId = comment.ParentCommentId,
                Replies = comment.Replies.Select(r => new CommentDTO
                {
                    Id = r.Id,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId,
                    UserName = r.User.UserName!,
                    LikeCount = r.Likes.Count,
                    ParentCommentId = r.ParentCommentId
                }).ToList()
            };

            return responseDTO;
        }

        public async Task UpdateCommentAsync(Guid commentId, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc, string currentUserId)
        {
            var comment = await _context.comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment ID: {commentId} not found.");
            }

            if (comment.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            var commentToPatch = new UpdateCommentDTO
            {
                Content = comment.Content
            };

            patchDoc.ApplyTo(commentToPatch);

            comment.Content = commentToPatch.Content;

            await _context.SaveChangesAsync();

        }
    }
}
