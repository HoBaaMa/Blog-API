using Blog_API.Data;
using Blog_API.DTOs;
using Blog_API.Models;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog_API.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Blog_API.Services.Implementation
{
    public class CommentService : ICommentService
    {
        private readonly BlogDbContext _context;
        private readonly IMapper _mapper;
        public CommentService(BlogDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
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


            return _mapper.Map<CommentDTO>(comment);
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
            //var comments = await _context.comments
            //    .Where(c => c.BlogPostId == blogPostId)
            //    .Include(c => c.Replies)
            //        .ThenInclude(r => r.User)
            //    .Include(c => c.Replies)
            //        .ThenInclude(r => r.Likes)
            //    .AsNoTracking()
            //    .ToListAsync();

            var comments = await _context.comments
                .AsNoTracking()
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
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
                .AsNoTracking()
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
            }

            return comment;
        }

        public async Task<CommentDTO> UpdateCommentAsync(Guid commentId, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc, string currentUserId)
        {
            var comment = await _context.comments
                .FirstOrDefaultAsync(c => c.Id == commentId);

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

            // Apply changes with processing error!
            patchDoc.ApplyTo(commentToPatch, error =>
            {
                throw new ArgumentException(error.ErrorMessage);
            });

            comment.Content = commentToPatch.Content;

            await _context.SaveChangesAsync();
            return _mapper.Map<CommentDTO>(comment);
        }
    }
}
