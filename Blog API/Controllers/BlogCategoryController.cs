using Blog_API.Models.DTOs;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog_API.Controllers
{
    /// <summary>
    /// Controller for managing blog categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BlogCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public BlogCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves all blog categories.
        /// </summary>
        /// <returns>Collection of all blog categories.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlogCategoryDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllBlogCategories()
        {
            var categories = await _categoryService.GetAllBlogCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Creates a new blog category.
        /// </summary>
        /// <param name="createBlogCategoryDTO">The category data.</param>
        /// <returns>The created category.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(BlogCategoryDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> CreateBlogCategory([FromBody] CreateBlogCategoryDTO createBlogCategoryDTO)
        {
            var category = await _categoryService.CreateBlogCategoryAsync(createBlogCategoryDTO);
            return Created($"/api/blogcategory", category);
        }

        /// <summary>
        /// Deletes a blog category.
        /// </summary>
        /// <param name="id">The category ID to delete.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBlogCategory(Guid id)
        {
            await _categoryService.DeleteBlogCategoryAsync(id);
            return NoContent();
        }
    }
}

