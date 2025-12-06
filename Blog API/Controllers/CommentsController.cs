using Blog_API.Models.DTOs;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Blog_API.Controllers
{
    /// <summary>
    /// Controller for managing comments on blog posts.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CommentsController : BaseApiController
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;
        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new comment on a blog post.
        /// </summary>
        /// <param name="commentDTO">The comment data.</param>
        /// <returns>The created comment with a Location header.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO commentDTO)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogInformation("API request to create comment by user {UserId} for blog post {BlogPostId}", 
                currentUserId, commentDTO.BlogPostId);

            var createdComment = await _commentService.CreateCommentAsync(commentDTO, currentUserId);
            
            _logger.LogInformation("Comment created successfully via API with ID {CommentId}", createdComment.Id);
            return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
        }

        /// <summary>
        /// Retrieves all comments for a specific blog post.
        /// </summary>
        /// <param name="id">The blog post ID.</param>
        /// <returns>Collection of comments for the blog post.</returns>
        [HttpGet("blogpost/{id:guid}")]
        [ProducesResponseType(typeof(IEnumerable<CommentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCommentsForBlogPost(Guid id)
        {
            _logger.LogInformation("API request to get all comments for blog post {BlogPostId}", id);
            
            var comments = await _commentService.GetAllCommentsForBlogPostAsync(id);
            
            _logger.LogInformation("Successfully retrieved comments for blog post {BlogPostId} via API", id);
            return Ok(comments);
        }

        /// <summary>
        /// Retrieves a comment by its unique identifier.
        /// </summary>
        /// <param name="id">The comment ID.</param>
        /// <returns>The comment if found.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            _logger.LogInformation("API request to get comment {CommentId}", id);
            
            var comment = await _commentService.GetCommentByIdAsync(id);
            
            _logger.LogInformation("Successfully retrieved comment {CommentId} via API", id);
            return Ok(comment);
        }

        /// <summary>
        /// Partially updates a comment using JSON Patch.
        /// </summary>
        /// <param name="id">The comment ID to update.</param>
        /// <param name="patchDoc">The JSON Patch document with updates.</param>
        /// <returns>The updated comment.</returns>
        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin, User")]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogInformation("API request to update comment {CommentId} by user {UserId}", id, currentUserId);

            var updatedComment = await _commentService.UpdateCommentAsync(id, patchDoc, currentUserId);
            
            _logger.LogInformation("Comment {CommentId} updated successfully via API", id);
            return Ok(updatedComment);
        }

        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="id">The comment ID to delete.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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

