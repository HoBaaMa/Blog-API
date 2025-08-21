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
        /// <summary>
        /// Initializes a new instance of <see cref="BlogPostsController"/> with the required dependencies.
        /// </summary>
        /// <remarks>
        /// Validates that injected dependencies are not null.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when a required dependency is null.</exception>
        public BlogPostsController(IBlogPostService blogPostService, ILogger<BlogPostsController> logger)
        {
            _blogPostService = blogPostService ?? throw new ArgumentNullException(nameof(blogPostService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all blog posts.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing a 200 OK response with the collection of blog posts.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await _blogPostService.GetAllBlogPostsAsync();
            return Ok(blogPosts);
        }

        /// <summary>
        /// Creates a new blog post for the authenticated user.
        /// </summary>
        /// <param name="blogPostDTO">Data for the blog post to create.</param>
        /// <returns>
        /// 201 Created with the created blog post in the response body and a Location header pointing to <see cref="GetBlogPostById(Guid)"/>.
        /// </returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostDTO blogPostDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var createdPost = await _blogPostService.CreateBlogPostAsync(blogPostDTO, currentUserId);

            return CreatedAtAction(nameof(GetBlogPostById), new { id = createdPost?.Id }, createdPost);
        }

        /// <summary>
        /// Retrieves a blog post by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the blog post to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the blog post in a 200 OK response.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBlogPostById(Guid id)
        {
            var blogPost = await _blogPostService.GetBlogPostByIdAsync(id);
            return Ok(blogPost);
        }

        /// <summary>
        /// Updates an existing blog post identified by <paramref name="id"/> with the supplied data.
        /// Requires an authenticated user.
        /// </summary>
        /// <param name="id">The GUID of the blog post to update (from route).</param>
        /// <param name="blogPostDTO">The blog post data to apply (from request body).</param>
        /// <returns>200 OK with the updated blog post model.</returns>
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlogPost (Guid id,[FromBody] CreateBlogPostDTO blogPostDTO)
        {
            var updatedBlogPost = await _blogPostService.UpdateBlogPostAsync(id, blogPostDTO);
            return Ok(updatedBlogPost);
        }
        /// <summary>
        /// Deletes the blog post identified by <paramref name="id"/> on behalf of the authenticated user.
        /// </summary>
        /// <param name="id">The GUID of the blog post to delete (from route).</param>
        /// <returns>An HTTP 204 No Content response when deletion succeeds.</returns>
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
