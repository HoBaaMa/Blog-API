using Blog_API.Models.DTOs;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly ILogger<LikesController> _logger;
        public LikesController(ILikeService likeService, ILogger<LikesController> logger)
        {
            _likeService = likeService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleLike([FromBody] CreateLikeDTO createLikeDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                bool likeResult = await _likeService.ToggleLikeAsync(userId, createLikeDTO.BlogPostId, createLikeDTO.CommentId);
                if (likeResult)
                {
                    if (createLikeDTO.BlogPostId.HasValue)
                    {
                        _logger.LogInformation($"User ID {userId} liked blog post ID: {createLikeDTO.BlogPostId}.");
                    }
                    else
                    {
                        _logger.LogInformation($"User ID {userId} liked comment ID: {createLikeDTO.CommentId}.");
                    }
                }
                else
                {
                    if (createLikeDTO.BlogPostId.HasValue)
                    {
                        _logger.LogInformation($"User ID {userId} unliked blog post ID: {createLikeDTO.BlogPostId}.");
                    }
                    else
                    {
                        _logger.LogInformation($"User ID {userId} unliked comment ID: {createLikeDTO.CommentId}.");
                    }
                }

                return likeResult ? Ok(new { message = "Liked." }) : Ok(new { message = "Unliked." });
            }
            catch(ArgumentException ex)
            {
                _logger.LogWarning(ex, "User did not supply neithr/either comment or/and blog post at the same time.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating like.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("blogpost")]
        public async Task<IActionResult> GetAllLikesByBlogPostId(Guid blogPostId)
        {
            var likes = await _likeService.GetAllLikesByBlogPostIdAsync(blogPostId);
            return Ok(likes);
        }

        [HttpGet("comment")]
        public async Task<IActionResult> GetAllLikesByCommentId(Guid commentId)
        {
            var likes = await _likeService.GetAllLikesByCommentIdAsync(commentId);
            return Ok(likes);
        }
    }
}
