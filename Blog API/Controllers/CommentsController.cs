using Blog_API.Models.DTOs;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : BaseApiController
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;
        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO commentDTO)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogInformation("API request to create comment by user {UserId} for blog post {BlogPostId}", 
                currentUserId, commentDTO.BlogPostId);

            var createdComment = await _commentService.CreateCommentAsync(commentDTO, currentUserId);
            
            _logger.LogInformation("Comment created successfully via API with ID {CommentId}", createdComment.Id);
            return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
        }

        [HttpGet("blogpost/{id:guid}")]
        public async Task<IActionResult> GetAllCommentsForBlogPost(Guid id)
        {
            _logger.LogInformation("API request to get all comments for blog post {BlogPostId}", id);
            
            var comments = await _commentService.GetAllCommentsForBlogPostAsync(id);
            
            _logger.LogInformation("Successfully retrieved comments for blog post {BlogPostId} via API", id);
            return Ok(comments);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            _logger.LogInformation("API request to get comment {CommentId}", id);
            
            var comment = await _commentService.GetCommentByIdAsync(id);
            
            _logger.LogInformation("Successfully retrieved comment {CommentId} via API", id);
            return Ok(comment);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogInformation("API request to update comment {CommentId} by user {UserId}", id, currentUserId);

            var updatedComment = await _commentService.UpdateCommentAsync(id, patchDoc, currentUserId);
            
            _logger.LogInformation("Comment {CommentId} updated successfully via API", id);
            return Ok(updatedComment);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogInformation("API request to delete comment {CommentId} by user {UserId}", id, currentUserId);

            await _commentService.DeleteCommentAsync(id, currentUserId);
            
            _logger.LogInformation("Comment {CommentId} deleted successfully via API", id);
            return NoContent();
        }
    }
}
