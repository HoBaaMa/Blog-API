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
        /// <summary>
        /// Initializes a new <see cref="CommentsController"/> with its required dependencies.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="commentService"/> or <paramref name="logger"/> is null.</exception>
        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Creates a new comment on a blog post for the currently authenticated user.
        /// </summary>
        /// <param name="commentDTO">The comment data transfer object containing the comment content and target blog post identifier.</param>
        /// <returns>201 Created with the created comment in the response body and a Location header targeting GetCommentById.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO commentDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var createdComment = await _commentService.CreateCommentAsync(commentDTO, currentUserId);

            return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
        }

        /// <summary>
        /// Retrieves all comments associated with the specified blog post.
        /// </summary>
        /// <param name="id">The GUID identifier of the blog post whose comments to retrieve.</param>
        /// <returns>200 OK with a collection of comments for the specified blog post.</returns>
        [HttpGet("blogpost/{id:guid}")]
        public async Task<IActionResult> GetAllCommentsForBlogPost(Guid id)
        {
            var comments = await _commentService.GetAllCommentsForBlogPostAsync(id);
            return Ok(comments);
        }

        /// <summary>
        /// Retrieves a comment by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the comment to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the comment (200 OK) when found.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            return Ok(comment);
        }

        /// <summary>
        /// Applies a JSON Patch to an existing comment and returns the updated comment.
        /// </summary>
        /// <remarks>
        /// Requires an authenticated user; the current user ID is taken from the caller's NameIdentifier claim.
        /// </remarks>
        /// <param name="id">The GUID of the comment to update.</param>
        /// <param name="patchDoc">The JSON Patch document describing the changes to apply to the comment DTO.</param>
        /// <returns>200 OK with the updated comment DTO.</returns>
        [HttpPatch("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc)
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var updatedComment = await _commentService.UpdateCommentAsync(id, patchDoc, currentUser);

            return Ok(updatedComment);
        }

        /// <summary>
        /// Deletes the comment with the specified ID on behalf of the authenticated user.
        /// </summary>
        /// <param name="id">The GUID of the comment to delete.</param>
        /// <returns>HTTP 204 No Content when the comment is successfully deleted.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment (Guid id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _commentService.DeleteCommentAsync(id, currentUserId);

            return NoContent();
        }
    }
}
