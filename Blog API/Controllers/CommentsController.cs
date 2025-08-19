using Blog_API.DTOs;
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
            _logger = logger;
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO commentDTO)
        {
            _logger.LogInformation("Creating a new comment.");
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var createdComment = await _commentService.CreateCommentAsync(commentDTO, currentUserId);

                _logger.LogInformation($"Comment created successfully with ID: {createdComment.Id}.");
                return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Parent comment with ID: {commentDTO.ParentCommentId} not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("blogpost/{id:guid}")]
        public async Task<IActionResult> GetAllCommentsForBlogPost(Guid id)
        {
            try
            {
                var comments = await _commentService.GetAllCommentsForBlogPostAsync(id);
                return Ok(comments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            _logger.LogInformation($"Retrieving comment with ID: {id}.");
            try
            {
                var comment = await _commentService.GetCommentByIdAsync(id);

                _logger.LogInformation($"Comment retrieved successfully: {comment!.Id}.");
                return Ok(comment);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Comment with ID: {id} not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving comment with ID: {id}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPatch("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc)
        {
            _logger.LogInformation($"Updating comment with ID: {id}.");
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var updatedComment = await _commentService.UpdateCommentAsync(id, patchDoc, currentUser);

                _logger.LogInformation($"Comment with ID: {id} updated successfully.");
                return Ok(updatedComment);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $": Unauthorized access attempt to update comment with ID: {id}.");
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Comment with ID: {id} not found for update.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating comment with ID: {id}.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment (Guid id)
        {
            _logger.LogInformation($"Deleting comment with ID: {id}.");
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                await _commentService.DeleteCommentAsync(id, currentUserId);

                _logger.LogInformation($"Comment with ID: {id} deleted successfully");
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Comment with ID: {id} not found for deletion.");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $": Unauthorized access attempt to update comment with ID: {id}.");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting comment with ID: {id}.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
