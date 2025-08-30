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
        /// <summary>
        /// Initializes a new instance of <see cref="BlogPostsController"/> with required dependencies.
        /// </summary>
        public BlogPostsController(IBlogPostService blogPostService, ILogger<BlogPostsController> logger)
        {
            _blogPostService = blogPostService ?? throw new ArgumentNullException(nameof(blogPostService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all blog posts.
        /// </summary>
        /// <summary>
        /// Retrieves all blog posts, optionally applying server-side filtering and sorting.
        /// </summary>
        /// <param name="filterOn">Optional name of the field to filter on (e.g., "title" or "author").</param>
        /// <param name="filterQuery">Optional filter value used to match the <paramref name="filterOn"/> field.</param>
        /// <param name="sortBy">Optional field name to sort the results by.</param>
        /// <param name="isAscending">If true (default), results are sorted ascending; set to false for descending.</param>
        /// <returns>An <see cref="IActionResult"/> containing a 200 OK response with the collection of blog posts.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts([FromQuery] string? filterOn,[FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending = true)
        {
            _logger.LogInformation("API request to get all blog posts");
            var blogPosts = await _blogPostService.GetAllBlogPostsAsync(filterOn, filterQuery, sortBy, isAscending);
            return Ok(blogPosts);
        }

        /// <summary>
        /// Creates a new blog post for the authenticated user.
        /// </summary>
        /// <param name="blogPostDTO">Data for the blog post to create.</param>
        /// <returns>
        /// 201 Created with the created blog post in the response body and a Location header pointing to <see cref="GetBlogPostById(Guid)"/>.
        /// <summary>
        /// Creates a new blog post for the authenticated user.
        /// </summary>
        /// <remarks>
        /// Requires an authenticated user; the creator's user id is read from the NameIdentifier claim and associated with the new post.
        /// </remarks>
        /// <param name="blogPostDTO">The blog post data from the request body (e.g., title, content, optional ImageUrls).</param>
        /// <returns>201 Created with the created blog post in the response body and a Location header pointing to GetBlogPostById.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostDTO blogPostDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            _logger.LogInformation("API request to create blog post by user {UserId} with {ImageCount} images", 
                currentUserId, blogPostDTO.ImageUrls?.Count ?? 0);
            

            var createdPost = await _blogPostService.CreateBlogPostAsync(blogPostDTO, currentUserId);
            return CreatedAtAction(nameof(GetBlogPostById), new { id = createdPost?.Id }, createdPost);
        }

        /// <summary>
        /// Retrieves a blog post by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the blog post to retrieve.</param>
        /// <summary>
        /// Retrieves a blog post by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the blog post to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> with the blog post wrapped in a 200 OK response.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBlogPostById(Guid id)
        {
            _logger.LogInformation("API request to get blog post {BlogPostId}", id);
            var blogPost = await _blogPostService.GetBlogPostByIdAsync(id);
            return Ok(blogPost);
        }

        /// <summary>
        /// Updates an existing blog post identified by <paramref name="id"/> with the supplied data.
        /// Requires an authenticated user.
        /// </summary>
        /// <param name="id">The GUID of the blog post to update (from route).</param>
        /// <param name="blogPostDTO">The blog post data to apply (from request body).</param>
        /// <summary>
        /// Updates an existing blog post using the provided data and returns the updated post.
        /// </summary>
        /// <param name="id">The GUID of the blog post to update.</param>
        /// <param name="blogPostDTO">The updated blog post payload (title, content, image URLs, etc.).</param>
        /// <returns>200 OK with the updated blog post.</returns>
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlogPost(Guid id, [FromBody] CreateBlogPostDTO blogPostDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            _logger.LogInformation("API request to update blog post {BlogPostId} by user {UserId} with {ImageCount} images", 
                id, currentUserId, blogPostDTO.ImageUrls?.Count ?? 0);
            
            var updatedBlogPost = await _blogPostService.UpdateBlogPostAsync(id, blogPostDTO, currentUserId);
            return Ok(updatedBlogPost);
        }
        /// <summary>
        /// Deletes the blog post with the specified <paramref name="id"/> on behalf of the authenticated user.
        /// </summary>
        /// <param name="id">The GUID of the blog post to delete.</param>
        /// <returns>HTTP 204 No Content on successful deletion.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            _logger.LogInformation("API request to delete blog post {BlogPostId} by user {UserId}", id, currentUserId);
            
            await _blogPostService.DeleteBlogPostAsync(id, currentUserId);
            return NoContent();
        }

        /// <summary>
        /// Retrieves paginated blog posts by category
        /// </summary>
        /// <param name="blogCategory">The blog category to filter by</param>
        /// <param name="paginationRequest">Pagination parameters including page number and page size</param>
        /// <summary>
        /// Retrieves blog posts for the specified category using the provided pagination parameters.
        /// </summary>
        /// <param name="blogCategory">The blog category to filter posts by.</param>
        /// <param name="paginationRequest">Pagination parameters (e.g., page number and page size) used to control the paged result.</param>
        /// <returns>200 OK with a paginated result containing matching blog posts and pagination metadata.</returns>
        [HttpGet("blogCategory")]
        public async Task<IActionResult> GetBlogPostsByCategory(
            [FromQuery] Models.Entities.BlogCategory blogCategory,
            [FromQuery] PaginationRequest paginationRequest)
        {
            _logger.LogInformation("API request to get paginated blog posts by category {BlogCategory}", blogCategory);

            var pagedResult = await _blogPostService.GetBlogPostsByCategoryAsync(blogCategory, paginationRequest);
            return Ok(pagedResult);
        }

        /// <summary>
        /// Retrieves all images for a specific blog post
        /// </summary>
        /// <param name="id">Blog post ID</param>
        /// <returns>Collection of image URLs for the blog post</returns>
        [HttpGet("{id:guid}/images")]
        public async Task<IActionResult> GetBlogPostImages(Guid id)
        {
            _logger.LogInformation("API request to get images for blog post {BlogPostId}", id);
            
            var images = await _blogPostService.GetBlogPostImagesAsync(id);
            
            _logger.LogInformation("Successfully retrieved images for blog post {BlogPostId} via API", id);
            return Ok(new { BlogPostId = id, Images = images, Count = images.Count });
        }
    }
}
