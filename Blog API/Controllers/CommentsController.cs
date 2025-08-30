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
        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsController"/> class with the required dependencies.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a required dependency is null.</exception>
        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Creates a new comment for a blog post on behalf of the authenticated user.
        /// </summary>
        /// <param name="commentDTO">The comment data including the target BlogPostId and content.</param>
        /// <returns>
        /// 201 Created with the created comment in the response body and a Location header pointing to <see cref="GetCommentById(Guid)"/>.
        /// </returns>
        /// <remarks>
        /// Requires an authenticated user (authorization enforced by the controller attribute).
        /// </remarks>
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
        /// Retrieves all comments for the specified blog post.
        /// </summary>
        /// <param name="id">The GUID of the blog post whose comments should be returned.</param>
        /// <returns>An <see cref="IActionResult"/> containing 200 OK with the list of comments for the blog post.</returns>
        [HttpGet("blogpost/{id:guid}")]
        public async Task<IActionResult> GetAllCommentsForBlogPost(Guid id)
        {
            _logger.LogInformation("API request to get all comments for blog post {BlogPostId}", id);
            
            var comments = await _commentService.GetAllCommentsForBlogPostAsync(id);
            
            _logger.LogInformation("Successfully retrieved comments for blog post {BlogPostId} via API", id);
            return Ok(comments);
        }

        /// <summary>
        /// Retrieves a comment by its GUID identifier.
        /// </summary>
        /// <param name="id">The GUID of the comment to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the comment model with HTTP 200 OK if found.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            _logger.LogInformation("API request to get comment {CommentId}", id);
            
            var comment = await _commentService.GetCommentByIdAsync(id);
            
            _logger.LogInformation("Successfully retrieved comment {CommentId} via API", id);
            return Ok(comment);
        }

        /// <summary>
        /// Applies a JSON Patch to an existing comment and returns the updated comment.
        /// </summary>
        /// <remarks>
        /// Requires an authenticated user; the operation is performed on behalf of the current user.
        /// </remarks>
        /// <param name="id">The GUID of the comment to update.</param>
        /// <param name="patchDoc">A JSON Patch document describing the changes to apply to the comment DTO.</param>
        /// <returns>200 OK with the updated comment if the update succeeds.</returns>
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
        /// Deletes a comment by its identifier.
        /// </summary>
        /// <remarks>
        /// Requires an authenticated user; the caller's user ID is taken from the NameIdentifier claim and used
        /// to authorize/perform the deletion via the comment service.
        /// </remarks>
        /// <param name="id">The GUID of the comment to delete.</param>
        /// <returns>An <see cref="IActionResult"/> that produces HTTP 204 No Content on successful deletion.</returns>
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
