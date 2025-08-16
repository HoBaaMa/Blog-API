using Blog_API.DTOs;
using Blog_API.Models;
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
        public async Task<IActionResult> CreareLike([FromBody] CreateLikeDTO createLikeDTO)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                bool likeResult = await _likeService.ToggleLikeAsync(createLikeDTO, user);
                if (likeResult)
                {
                    if (createLikeDTO.BlogPostId.HasValue)
                    {
                        _logger.LogInformation($"User ID {user} liked blog post ID: {createLikeDTO.BlogPostId}.");
                    }
                    else
                    {
                        _logger.LogInformation($"User ID {user} liked comment ID: {createLikeDTO.CommentId}.");
                    }
                }
                else
                {
                    if (createLikeDTO.BlogPostId.HasValue)
                    {
                        _logger.LogInformation($"User ID {user} unliked blog post ID: {createLikeDTO.BlogPostId}.");
                    }
                    else
                    {
                        _logger.LogInformation($"User ID {user} unliked comment ID: {createLikeDTO.CommentId}.");
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
    }
}
