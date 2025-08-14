using Blog_API.DTOs;
using Blog_API.Models;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "No blog posts found.");
                return NotFound("No blog posts found: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                _logger.LogError(ex, "An error occurred while fetching blog posts.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostDTO blogPostDTO)
        {
            _logger.LogInformation("Creating a new blog post.");
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                try
                {
                    var createdPost = await _blogPostService.CreateBlogPostAsync(blogPostDTO, user.Id);
                    _logger.LogInformation($"Blog post created successfully");
                    return CreatedAtAction(nameof(GetBlogPostById), new { id = createdPost?.Id }, createdPost);
                }
                catch (ArgumentNullException ex)
                {
                    _logger.LogError(ex, "Invalid blog post data.");
                    return BadRequest("Invalid blog post data: " + ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating the blog post.");
                    // Log the exception (not shown here for brevity)
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Unauthorized attempt to create a blog post.");
                return Unauthorized("User not authenticated.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogPostById(Guid id)
        {
            try
            {
                var blogPost = await _blogPostService.GetBlogPostByIdAsync(id);
                return Ok(blogPost);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateBlogPost (Guid id, CreateBlogPostDTO blogPostDTO)
        {
            _logger.LogInformation($"Updating blog post with ID: {id}");
            try
            {
                await _blogPostService.UpdateBlogPostAsync(id, blogPostDTO);
                _logger.LogInformation($"Blog post with ID: {id} updated successfully.");
                return Ok("Blog post updated successfully.");
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Invalid blog post data.");
                return BadRequest("Invalid blog post data: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the blog post.");
                // Log the exception (not shown here for brevity)
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
                await _blogPostService.DeleteBlogPostAsync(id);
                _logger.LogInformation($"Blog post with ID: {id} deleted successfully.");
                return Ok("Blog post deleted successfully.");
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Blog post not found.");
                return NotFound("Blog post not found: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the blog post.");
                // Log the exception (not shown here for brevity)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
