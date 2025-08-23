using Blog_API.Models.Entities;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Blog_API.Models.DTOs;
using Blog_API.Repositories.Interfaces;

namespace Blog_API.Services.Implementation
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        public CommentService(IMapper mapper, ICommentRepository commentRepository, IBlogPostRepository blogPostRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(mapper));
            _blogPostRepository = blogPostRepository;
        }
        
        public async Task<CommentDTO> CreateCommentAsync(CreateCommentDTO commentDTO, string userId)
        {
            var comment = _mapper.Map<Comment>(commentDTO);
            comment.UserId = userId;

            /*
             * Brainstorming:
             * ParentCommentId == null => means it's top-level comment
             * ParenctCommentId != null => means it's a reply to the parent comment with that ID
             * EF Core will handle the relationship automatically and add the reply comment to the Replies collection of the parent comment.
             */

            if (commentDTO.ParentCommentId.HasValue)
            {
                bool parentExists = await _commentRepository.IsParentExistsAsync(commentDTO.ParentCommentId!.Value, commentDTO.BlogPostId);
                if (!parentExists)
                {
                    throw new KeyNotFoundException($"Parent comment with ID: {commentDTO.ParentCommentId.Value} not found.");
                }
            }

            await _commentRepository.AddAsync(comment);
            var createdComment = await _commentRepository.GetByIdAsync(comment.Id);

            return _mapper.Map<CommentDTO>(createdComment);
        }

        public async Task DeleteCommentAsync(Guid commentId, string currentUserId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);

            // TODO: Error Flow
            if (comment?.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException();
            }
            else if (comment == null)
            {
                throw new KeyNotFoundException($"Comment ID: {commentId} not found.");
            }

            await _commentRepository.DeleteAsync(comment); 
        }

        // TODO: Reply User is null
        public async Task<IReadOnlyCollection<CommentDTO>> GetAllCommentsForBlogPostAsync(Guid blogPostId)
        {
            if (await _blogPostRepository.GetByIdAsync(blogPostId) == null)
            {
                throw new KeyNotFoundException($"Blog Post with ID {blogPostId} not found.");
            }

            var comments = await _commentRepository.GetAllForBlogPostAsync(blogPostId);

            if (comments.Count() == 0)
            {
                throw new KeyNotFoundException($"No Comments found for Blog Post ID {blogPostId}.");
            }

            return _mapper.Map<IReadOnlyCollection<CommentDTO>>(comments);
        }

        public async Task<CommentDTO?> GetCommentByIdAsync(Guid commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
            }

            return _mapper.Map<CommentDTO>(comment);
        }

        public async Task<CommentDTO> UpdateCommentAsync(Guid commentId, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc, string currentUserId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);

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

            
            patchDoc.ApplyTo(commentToPatch);
            comment.Content = commentToPatch.Content;

            await _commentRepository.UpdateAsync(comment);

            return _mapper.Map<CommentDTO>(comment);
        }
    }
}
