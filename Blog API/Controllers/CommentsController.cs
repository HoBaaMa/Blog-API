using Blog_API.Models.DTOs;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;
        
        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Creates a new comment or reply to an existing comment
        /// </summary>
        /// <param name="commentDTO">Comment data including content and optional parent comment ID</param>
        /// <returns>Created comment details</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO commentDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            _logger.LogInformation("API request to create comment by user {UserId} for blog post {BlogPostId}", 
                currentUserId, commentDTO.BlogPostId);

            var createdComment = await _commentService.CreateCommentAsync(commentDTO, currentUserId);
            
            _logger.LogInformation("Comment created successfully via API with ID {CommentId}", createdComment.Id);
            return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
        }

        /// <summary>
        /// Retrieves all comments for a specific blog post
        /// </summary>
        /// <param name="id">Blog post ID</param>
        /// <returns>Collection of comments for the blog post</returns>
        [HttpGet("blogpost/{id:guid}")]
        public async Task<IActionResult> GetAllCommentsForBlogPost(Guid id)
        {
            _logger.LogInformation("API request to get all comments for blog post {BlogPostId}", id);
            
            var comments = await _commentService.GetAllCommentsForBlogPostAsync(id);
            
            _logger.LogInformation("Successfully retrieved comments for blog post {BlogPostId} via API", id);
            return Ok(comments);
        }

        /// <summary>
        /// Retrieves a specific comment by ID
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <returns>Comment details</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            _logger.LogInformation("API request to get comment {CommentId}", id);
            
            var comment = await _commentService.GetCommentByIdAsync(id);
            
            _logger.LogInformation("Successfully retrieved comment {CommentId} via API", id);
            return Ok(comment);
        }

        /// <summary>
        /// Updates a comment using JSON Patch operations
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <param name="patchDoc">JSON Patch document with update operations</param>
        /// <returns>Updated comment details</returns>
        [HttpPatch("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _logger.LogInformation("API request to update comment {CommentId} by user {UserId}", id, currentUserId);

            var updatedComment = await _commentService.UpdateCommentAsync(id, patchDoc, currentUserId);
            
            _logger.LogInformation("Comment {CommentId} updated successfully via API", id);
            return Ok(updatedComment);
        }

        /// <summary>
        /// Deletes a comment
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <returns>No content on successful deletion</returns>
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _logger.LogInformation("API request to delete comment {CommentId} by user {UserId}", id, currentUserId);

            await _commentService.DeleteCommentAsync(id, currentUserId);
            
            _logger.LogInformation("Comment {CommentId} deleted successfully via API", id);
            return NoContent();
        }
    }
}
