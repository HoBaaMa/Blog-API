using Blog_API.DTOs;
using Blog_API.Models;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BlogPostsController> _logger;
        public BlogPostsController(IBlogPostService blogPostService, UserManager<ApplicationUser> userManager, ILogger<BlogPostsController> logger)
        {
            _blogPostService = blogPostService ?? throw new ArgumentNullException(nameof(blogPostService));
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            _logger.LogInformation("Fetching all blog posts.");
            try
            {
                var blogPosts = await _blogPostService.GetAllBlogPostsAsync();
                _logger.LogInformation($"Retrieved {blogPosts.Count()} blog posts.");
                return Ok(blogPosts);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "No blog posts found.");
                return NotFound("No blog posts found: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching blog posts.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostDTO blogPostDTO)
        {
            _logger.LogInformation("Creating a new blog post.");
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var createdPost = await _blogPostService.CreateBlogPostAsync(blogPostDTO, currentUserId);

                _logger.LogInformation($"Blog post created successfully");
                return CreatedAtAction(nameof(GetBlogPostById), new { id = createdPost?.Id }, createdPost);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the blog post.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogPostById(Guid id)
        {
            _logger.LogInformation($"Fetching blod post ID: {id}");
            try
            {
                var blogPost = await _blogPostService.GetBlogPostByIdAsync(id);
                _logger.LogInformation($"Blog post with ID: {id} retrieved successfully.");
                return Ok(blogPost);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blog post not found.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlogPost (Guid id, CreateBlogPostDTO blogPostDTO)
        {
            _logger.LogInformation($"Updating blog post with ID: {id}");
            try
            {
                await _blogPostService.UpdateBlogPostAsync(id, blogPostDTO);
                _logger.LogInformation($"Blog post with ID: {id} updated successfully.");
                return Ok("Blog post updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Blog post not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the blog post.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteBlogPost (Guid id)
        {
            _logger.LogInformation($"Deleting blog post with ID: {id}");
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _blogPostService.DeleteBlogPostAsync(id, currentUserId);

                _logger.LogInformation($"Blog post with ID: {id} deleted successfully.");
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, $": Unauthorized access attempt to delete blog post with ID: {id}");
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Blog post not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the blog post.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
