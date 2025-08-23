using Blog_API.Models.DTOs;
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
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var updatedBlogPost = await _blogPostService.UpdateBlogPostAsync(id, blogPostDTO, currentUserId);

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

        /// <summary>
        /// Retrieves blog posts by category without pagination
        /// </summary>
        /// <param name="blogCategory">The blog category to filter by</param>
        /// <returns>Collection of blog posts in the specified category</returns>
        [HttpGet("blogCategory")]
        public async Task<IActionResult> GetBlogPostsByCategory(Models.Entities.BlogCategory blogCategory)
        {
            var blogPosts = await _blogPostService.GetBlogPostsByCategoryAsync(blogCategory);

            return Ok(blogPosts);
        }

        /// <summary>
        /// Retrieves paginated blog posts by category
        /// </summary>
        /// <param name="blogCategory">The blog category to filter by</param>
        /// <param name="paginationRequest">Pagination parameters including page number and page size</param>
        /// <returns>Paginated result containing blog posts and pagination metadata</returns>
        [HttpGet("blogCategory/paged")]
        public async Task<IActionResult> GetBlogPostsByCategoryPaged(
            [FromQuery] Models.Entities.BlogCategory blogCategory,
            [FromQuery] PaginationRequest paginationRequest)
        {
            var pagedResult = await _blogPostService.GetBlogPostsByCategoryPagedAsync(blogCategory, paginationRequest);

            return Ok(pagedResult);
        }
    }
}
