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
        private readonly ILogger<CommentService> _logger;
        
        public CommentService(IMapper mapper, ICommentRepository commentRepository, IBlogPostRepository blogPostRepository, ILogger<CommentService> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<CommentDTO> CreateCommentAsync(CreateCommentDTO commentDTO, string userId)
        {
            /*
            * Brainstorming:
            * ParentCommentId == null => means it's top-level comment
            * ParentCommentId != null => means it's a reply to the parent comment with that ID
            * EF Core will handle the relationship automatically and add the reply comment to the Replies collection of the parent comment.
            */
            _logger.LogInformation("Creating comment for user {UserId} on blog post {BlogPostId}", userId, commentDTO.BlogPostId);

            try
            {
                var comment = _mapper.Map<Comment>(commentDTO);
                comment.UserId = userId;

                // Check if this is a reply to another comment
                if (commentDTO.ParentCommentId.HasValue)
                {
                    _logger.LogDebug("Checking if parent comment {ParentCommentId} exists for blog post {BlogPostId}", 
                        commentDTO.ParentCommentId.Value, commentDTO.BlogPostId);
                    
                    bool parentExists = await _commentRepository.IsParentExistsAsync(commentDTO.ParentCommentId.Value, commentDTO.BlogPostId);
                    if (!parentExists)
                    {
                        _logger.LogWarning("Parent comment {ParentCommentId} not found for blog post {BlogPostId}", 
                            commentDTO.ParentCommentId.Value, commentDTO.BlogPostId);
                        throw new KeyNotFoundException($"Parent comment with ID: {commentDTO.ParentCommentId.Value} not found.");
                    }
                    _logger.LogDebug("Parent comment {ParentCommentId} found and valid", commentDTO.ParentCommentId.Value);
                }
                else
                {
                    _logger.LogDebug("Creating top-level comment (no parent)");
                }

                await _commentRepository.AddAsync(comment);
                _logger.LogInformation("Comment created successfully with ID {CommentId} for user {UserId}", comment.Id, userId);

                var createdComment = await _commentRepository.GetByIdAsync(comment.Id);
                return _mapper.Map<CommentDTO>(createdComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment for user {UserId} on blog post {BlogPostId}", userId, commentDTO.BlogPostId);
                throw;
            }
        }

        public async Task DeleteCommentAsync(Guid commentId, string currentUserId)
        {
            _logger.LogInformation("Attempting to delete comment {CommentId} by user {UserId}", commentId, currentUserId);

            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);

                if (comment == null)
                {
                    _logger.LogWarning("Comment {CommentId} not found for deletion", commentId);
                    throw new KeyNotFoundException($"Comment ID: {commentId} not found.");
                }

                if (comment.UserId != currentUserId)
                {
                    _logger.LogWarning("Unauthorized deletion attempt for comment {CommentId} by user {UserId}. Comment owner: {OwnerUserId}", 
                        commentId, currentUserId, comment.UserId);
                    throw new UnauthorizedAccessException();
                }

                await _commentRepository.DeleteAsync(comment);
                _logger.LogInformation("Comment {CommentId} deleted successfully by user {UserId}", commentId, currentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment {CommentId} by user {UserId}", commentId, currentUserId);
                throw;
            }
        }

        public async Task<IReadOnlyCollection<CommentDTO>> GetAllCommentsForBlogPostAsync(Guid blogPostId)
        {
            _logger.LogInformation("Retrieving all comments for blog post {BlogPostId}", blogPostId);

            try
            {
                // Verify blog post exists first
                var blogPost = await _blogPostRepository.GetByIdAsync(blogPostId);
                if (blogPost == null)
                {
                    _logger.LogWarning("Blog post {BlogPostId} not found when retrieving comments", blogPostId);
                    throw new KeyNotFoundException($"Blog Post with ID {blogPostId} not found.");
                }

                var comments = await _commentRepository.GetAllForBlogPostAsync(blogPostId);
                _logger.LogInformation("Retrieved {CommentCount} comments for blog post {BlogPostId}", comments.Count, blogPostId);

                if (comments.Count == 0)
                {
                    _logger.LogInformation("No comments found for blog post {BlogPostId}", blogPostId);
                    throw new KeyNotFoundException($"No Comments found for Blog Post ID {blogPostId}.");
                }

                return _mapper.Map<IReadOnlyCollection<CommentDTO>>(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving comments for blog post {BlogPostId}", blogPostId);
                throw;
            }
        }

        public async Task<CommentDTO?> GetCommentByIdAsync(Guid commentId)
        {
            _logger.LogInformation("Retrieving comment {CommentId}", commentId);

            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);

                if (comment == null)
                {
                    _logger.LogWarning("Comment {CommentId} not found", commentId);
                    throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
                }

                _logger.LogInformation("Comment {CommentId} retrieved successfully", commentId);
                return _mapper.Map<CommentDTO>(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving comment {CommentId}", commentId);
                throw;
            }
        }

        public async Task<CommentDTO> UpdateCommentAsync(Guid commentId, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc, string currentUserId)
        {
            _logger.LogInformation("Updating comment {CommentId} by user {UserId}", commentId, currentUserId);

            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);

                if (comment == null)
                {
                    _logger.LogWarning("Comment {CommentId} not found for update", commentId);
                    throw new KeyNotFoundException($"Comment ID: {commentId} not found.");
                }

                if (comment.UserId != currentUserId)
                {
                    _logger.LogWarning("Unauthorized update attempt for comment {CommentId} by user {UserId}. Comment owner: {OwnerUserId}", 
                        commentId, currentUserId, comment.UserId);
                    throw new UnauthorizedAccessException();
                }

                var originalContent = comment.Content;
                var commentToPatch = new UpdateCommentDTO
                {
                    Content = comment.Content
                };

                _logger.LogDebug("Applying patch operations to comment {CommentId}", commentId);
                patchDoc.ApplyTo(commentToPatch);
                comment.Content = commentToPatch.Content;

                await _commentRepository.UpdateAsync(comment);
                
                _logger.LogInformation("Comment {CommentId} updated successfully by user {UserId}. Content changed from '{OriginalContent}' to '{NewContent}'", 
                    commentId, currentUserId, originalContent, comment.Content);

                return _mapper.Map<CommentDTO>(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment {CommentId} by user {UserId}", commentId, currentUserId);
                throw;
            }
        }
    }
}
