using Blog_API.Models.DTOs;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : BaseApiController
    {
        private readonly ILikeService _likeService;
        private readonly ILogger<LikesController> _logger;
        
        public LikesController(ILikeService likeService, ILogger<LikesController> logger)
        {
            _likeService = likeService ?? throw new ArgumentNullException(nameof(likeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Toggles like/unlike on a blog post or comment
        /// </summary>
        /// <param name="createLikeDTO">Like data specifying either blog post ID or comment ID</param>
        /// <returns>Result indicating whether item was liked or unliked</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> ToggleLike([FromBody] CreateLikeDTO createLikeDTO)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("API request to toggle like by user {UserId}", userId);

            bool likeResult = await _likeService.ToggleLikeAsync(userId, createLikeDTO.BlogPostId, createLikeDTO.CommentId);
            
            var action = likeResult ? "liked" : "unliked";
            
            return Ok(new { message = likeResult ? "Liked." : "Unliked." });
        }

        /// <summary>
        /// Retrieves all likes for a specific blog post
        /// </summary>
        /// <param name="blogPostId">Blog post ID</param>
        /// <returns>Collection of likes for the blog post</returns>
        [HttpGet("blogpost")]
        public async Task<IActionResult> GetAllLikesByBlogPostId(Guid blogPostId)
        {
            _logger.LogInformation("API request to get all likes for blog post {BlogPostId}", blogPostId);
            var likes = await _likeService.GetAllLikesByBlogPostIdAsync(blogPostId);
            
            return Ok(likes);
        }

        /// <summary>
        /// Retrieves all likes for a specific comment
        /// </summary>
        /// <param name="commentId">Comment ID</param>
        /// <returns>Collection of likes for the comment</returns>
        [HttpGet("comment")]
        public async Task<IActionResult> GetAllLikesByCommentId(Guid commentId)
        {
            _logger.LogInformation("API request to get all likes for comment {CommentId}", commentId);
            var likes = await _likeService.GetAllLikesByCommentIdAsync(commentId);
            
            return Ok(likes);
        }
    }
}
