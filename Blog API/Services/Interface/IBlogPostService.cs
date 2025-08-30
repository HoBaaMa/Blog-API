using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

namespace Blog_API.Services.Interface
{
    public interface IBlogPostService
    {
        /// <summary>
/// Retrieves all blog posts, optionally filtered and/or sorted.
/// </summary>
/// <param name="filterOn">Optional name of the post property to filter on (e.g., "Title", "Author"). If null no filtering is applied.</param>
/// <param name="filterQuery">Optional filter value to match against the <paramref name="filterOn"/> property. Ignored when <paramref name="filterOn"/> is null.</param>
/// <param name="sortBy">Optional name of the post property to sort by (e.g., "CreatedAt", "Title"). If null the default ordering is used.</param>
/// <param name="isAscending">If true (default), sort in ascending order; if false, sort in descending order. Ignored when <paramref name="sortBy"/> is null.</param>
/// <returns>A read-only collection of <see cref="BlogPostDTO"/> matching the optional filter and sort criteria.</returns>
Task<IReadOnlyCollection<BlogPostDTO>> GetAllBlogPostsAsync(string? filterOn, string? filterQuery, string? sortBy, bool? isAscending = true);
        /// <summary>
/// Retrieves blog posts belonging to the specified category, returned as a paged result.
/// </summary>
/// <param name="blogCategory">The category to filter blog posts by.</param>
/// <param name="paginationRequest">Pagination parameters (page number, page size, etc.) controlling which page of results to return.</param>
/// <returns>A task that resolves to a <see cref="PagedResult{BlogPostDTO}"/> containing the requested page of blog post DTOs for the specified category.</returns>
Task<PagedResult<BlogPostDTO>> GetBlogPostsByCategoryAsync(BlogCategory blogCategory, PaginationRequest paginationRequest);
        /// <summary>
/// Retrieves a single blog post by its unique identifier.
/// </summary>
/// <param name="id">The GUID of the blog post to retrieve.</param>
/// <returns>The matching <see cref="BlogPostDTO"/>, or <c>null</c> if no post exists with the provided id.</returns>
Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id);
        /// <summary>
/// Creates a new blog post from the provided DTO and associates it with the given user.
/// </summary>
/// <param name="blogPostDTO">Data transfer object containing the content, title, category, and other input fields for the new post.</param>
/// <param name="userId">Identifier of the user creating the post; used as the post's author/owner.</param>
/// <returns>The created blog post represented as a <see cref="BlogPostDTO"/>, including persisted fields such as its ID and any server-generated metadata.</returns>
Task<BlogPostDTO> CreateBlogPostAsync(CreateBlogPostDTO blogPostDTO, string userId);
        Task<BlogPostDTO> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO, string currentUserId);
        Task DeleteBlogPostAsync(Guid id, string currentUserId);
        Task<IReadOnlyCollection<string>> GetBlogPostImagesAsync(Guid blogPostId);
    }
}
