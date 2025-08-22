using Blog_API.Data;
using Blog_API.Models.Entities;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog_API.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Blog_API.Models.DTOs;

namespace Blog_API.Services.Implementation
{
    public class CommentService : ICommentService
    {
        private readonly BlogDbContext _context;
        private readonly IMapper _mapper;
        public CommentService(BlogDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                bool parentExists = await _context.Comments.AnyAsync(c => c.Id == commentDTO.ParentCommentId.Value && c.BlogPostId == commentDTO.BlogPostId);
                if (!parentExists)
                {
                    throw new KeyNotFoundException($"Parent comment with ID: {commentDTO.ParentCommentId.Value} not found.");
                }
            }
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();


            return _mapper.Map<CommentDTO>(comment);
        }

        public async Task DeleteCommentAsync(Guid commentId, string currentUserId)
        {
            var isOwner = await _context.Comments.AnyAsync(c => c.Id == commentId && c.UserId == currentUserId);
            if (!isOwner)
            {
                bool exists = await _context.Comments.AnyAsync(c => c.Id == commentId);
                throw exists ? new UnauthorizedAccessException() : new KeyNotFoundException($"Comment ID: {commentId} not found.");
            }
            _context.Comments.Remove(new Comment { Id = commentId});
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

            if (!await _context.BlogPosts.AnyAsync(bp => bp.Id == blogPostId))
            {
                throw new KeyNotFoundException($"Blog Post with ID {blogPostId} not found.");
            }

            var Comments = await _context.Comments
                .AsNoTracking()
                .Where(c => c.BlogPostId == blogPostId && c.ParentCommentId == null)
                .Include(u => u.User)
                .Include(r => r.Replies)
                    .ThenInclude(u => u.User)
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (Comments.Count() == 0)
            {
                throw new KeyNotFoundException($"No Comments found for Blog Post ID {blogPostId}.");
            }

            return Comments;
        }

        public async Task<CommentDTO?> GetCommentByIdAsync(Guid commentId)
        {
            var comment = await _context.Comments
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
            var comment = await _context.Comments
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment ID: {commentId} not found.");
            }

            if (comment.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException();
            }

            var commentToPatch = new UpdateCommentDTO
            {
                Content = comment.Content
            };

            // Apply changes with processing error!
            patchDoc.ApplyTo(commentToPatch, error =>
            {
                throw new ArgumentException();
            });

            comment.Content = commentToPatch.Content;

            await _context.SaveChangesAsync();
            return _mapper.Map<CommentDTO>(comment);
        }
    }
}
