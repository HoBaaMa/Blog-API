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
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO commentDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var createdComment = await _commentService.CreateCommentAsync(commentDTO, currentUserId);

            return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
        }

        [HttpGet("blogpost/{id:guid}")]
        public async Task<IActionResult> GetAllCommentsForBlogPost(Guid id)
        {
            var comments = await _commentService.GetAllCommentsForBlogPostAsync(id);
            return Ok(comments);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            return Ok(comment);
        }

        [HttpPatch("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] JsonPatchDocument<UpdateCommentDTO> patchDoc)
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var updatedComment = await _commentService.UpdateCommentAsync(id, patchDoc, currentUser);

            return Ok(updatedComment);
        }

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
