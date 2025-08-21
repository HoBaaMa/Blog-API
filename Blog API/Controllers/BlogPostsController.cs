using Blog_API.DTOs;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<BlogPostsController> _logger;
        public BlogPostsController(IBlogPostService blogPostService, ILogger<BlogPostsController> logger)
        {
            _blogPostService = blogPostService ?? throw new ArgumentNullException(nameof(blogPostService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await _blogPostService.GetAllBlogPostsAsync();
            return Ok(blogPosts);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostDTO blogPostDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var createdPost = await _blogPostService.CreateBlogPostAsync(blogPostDTO, currentUserId);

            return CreatedAtAction(nameof(GetBlogPostById), new { id = createdPost?.Id }, createdPost);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBlogPostById(Guid id)
        {
            var blogPost = await _blogPostService.GetBlogPostByIdAsync(id);
            return Ok(blogPost);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlogPost (Guid id,[FromBody] CreateBlogPostDTO blogPostDTO)
        {
            var updatedBlogPost = await _blogPostService.UpdateBlogPostAsync(id, blogPostDTO);
            return Ok(updatedBlogPost);
        }
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlogPost (Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _blogPostService.DeleteBlogPostAsync(id, currentUserId);

            return NoContent();
        }
    }
}
