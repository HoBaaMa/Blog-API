using Blog_API.Models.DTOs;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog_API.Controllers
{
    /// <summary>
    /// Controller for managing blog posts.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BlogPostsController : BaseApiController
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<BlogPostsController> _logger;
        public BlogPostsController(IBlogPostService blogPostService, ILogger<BlogPostsController> logger)
        {
            _blogPostService = blogPostService ?? throw new ArgumentNullException(nameof(blogPostService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all blog posts with optional filtering, sorting, and pagination.
        /// </summary>
        /// <param name="paginationRequest">Pagination parameters.</param>
        /// <param name="filterOn">Property name to filter on.</param>
        /// <param name="filterQuery">Filter value to match.</param>
        /// <param name="sortBy">Property name to sort by.</param>
        /// <param name="isAscending">Sort direction (true for ascending).</param>
        /// <returns>Paginated collection of blog posts.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<BlogPostDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBlogPosts([FromQuery] PaginationRequest paginationRequest, [FromQuery] string? filterOn,[FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending = true)
        {
            _logger.LogInformation("API request to get all blog posts");
            var blogPosts = await _blogPostService.GetAllBlogPostsAsync(paginationRequest,filterOn, filterQuery, sortBy, isAscending);
            return Ok(blogPosts);
        }

        /// <summary>
        /// Creates a new blog post for the authenticated user.
        /// </summary>
        /// <param name="blogPostDTO">Data for the blog post to create.</param>
        /// <returns>The created blog post with a Location header.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostDTO blogPostDTO)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogInformation("API request to create blog post by user {UserId} with {ImageCount} images", 
                currentUserId, blogPostDTO.ImageUrls?.Count ?? 0);
            

            var createdPost = await _blogPostService.CreateBlogPostAsync(blogPostDTO, currentUserId);
            return CreatedAtAction(nameof(GetBlogPostById), new { id = createdPost?.Id }, createdPost);
        }

        /// <summary>
        /// Retrieves a blog post by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the blog post to retrieve.</param>
        /// <returns>The blog post if found.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBlogPostById(Guid id)
        {
            _logger.LogInformation("API request to get blog post {BlogPostId}", id);
            var blogPost = await _blogPostService.GetBlogPostByIdAsync(id);
            return Ok(blogPost);
        }

        /// <summary>
        /// Updates an existing blog post.
        /// </summary>
        /// <param name="id">The GUID of the blog post to update.</param>
        /// <param name="blogPostDTO">The updated blog post data.</param>
        /// <returns>The updated blog post.</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBlogPost(Guid id, [FromBody] CreateBlogPostDTO blogPostDTO)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogInformation("API request to update blog post {BlogPostId} by user {UserId} with {ImageCount} images", 
                id, currentUserId, blogPostDTO.ImageUrls?.Count ?? 0);
            
            var updatedBlogPost = await _blogPostService.UpdateBlogPostAsync(id, blogPostDTO, currentUserId);
            return Ok(updatedBlogPost);
        }

        /// <summary>
        /// Deletes a blog post.
        /// </summary>
        /// <param name="id">The GUID of the blog post to delete.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogInformation("API request to delete blog post {BlogPostId} by user {UserId}", id, currentUserId);
            
            await _blogPostService.DeleteBlogPostAsync(id, currentUserId);
            return NoContent();
        }

        /// <summary>
        /// Retrieves paginated blog posts by category.
        /// </summary>
        /// <param name="blogCategory">The blog category to filter by.</param>
        /// <param name="paginationRequest">Pagination parameters.</param>
        /// <returns>Paginated result containing blog posts.</returns>
        [HttpGet("blogCategory")]
        [ProducesResponseType(typeof(PagedResult<BlogPostDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBlogPostsByCategory(
            [FromQuery] Models.Entities.BlogCategory blogCategory,
            [FromQuery] PaginationRequest paginationRequest)
        {
            _logger.LogInformation("API request to get paginated blog posts by category {BlogCategory}", blogCategory);

            var pagedResult = await _blogPostService.GetBlogPostsByCategoryAsync(blogCategory, paginationRequest);
            return Ok(pagedResult);
        }

        /// <summary>
        /// Retrieves all images for a specific blog post.
        /// </summary>
        /// <param name="id">Blog post ID.</param>
        /// <returns>Collection of image URLs for the blog post.</returns>
        [HttpGet("{id:guid}/images")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBlogPostImages(Guid id)
        {
            _logger.LogInformation("API request to get images for blog post {BlogPostId}", id);
            
            var images = await _blogPostService.GetBlogPostImagesAsync(id);
            
            _logger.LogInformation("Successfully retrieved images for blog post {BlogPostId} via API", id);
            return Ok(new { BlogPostId = id, Images = images, Count = images.Count });
        }
    }
}

