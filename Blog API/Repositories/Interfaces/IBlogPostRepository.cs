using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Blog_API.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        /// <summary>
/// Retrieve a single BlogPost by its unique identifier.
/// </summary>
/// <param name="id">The GUID of the blog post to retrieve.</param>
/// <returns>
/// A task that resolves to the <see cref="BlogPost"/> with the specified id, or <c>null</c> if no matching post exists.
/// </returns>
Task<BlogPost?> GetByIdAsync(Guid id);
        /// <summary>
/// Retrieves all BlogPost records, optionally filtered and sorted.
/// </summary>
/// <param name="filterOn">Optional name of the BlogPost property to filter on. If null, no filtering is applied.</param>
/// <param name="filterQuery">Optional filter value to match against the <paramref name="filterOn"/> property. Ignored if <paramref name="filterOn"/> is null.</param>
/// <param name="sortBy">Optional name of the BlogPost property to sort by. If null, the default ordering is used by the implementation.</param>
/// <param name="isAscending">Optional sort direction. When true (default), results are returned in ascending order; when false, in descending order. Ignored if <paramref name="sortBy"/> is null.</param>
/// <returns>A read-only collection of BlogPost objects that satisfy the optional filter and sort criteria.</returns>
Task<IReadOnlyCollection<BlogPost>> GetAllAsync(string? filterOn, string? filterQuery, string? sortBy, bool? isAscending = true);
        /// <summary>
/// Retrieves a page of blog posts for the specified category.
/// </summary>
/// <param name="blogCategory">The category of blog posts to retrieve.</param>
/// <param name="paginationRequest">Pagination parameters (page number, page size, etc.) for the query.</param>
/// <returns>
/// A tuple containing:
/// - <c>blogPosts</c>: a read-only collection of BlogPost for the requested page;
/// - <c>totalCount</c>: the total number of BlogPost records in the given category.
/// </returns>
Task<(IReadOnlyCollection<BlogPost> blogPosts, int totalCount)> GetBlogPostsByCategoryAsync(BlogCategory blogCategory, PaginationRequest paginationRequest);
        /// <summary>
/// Adds a new BlogPost to the repository asynchronously.
/// </summary>
/// <param name="blogPost">The BlogPost entity to add. The caller should provide a fully constructed entity to be persisted.</param>
Task AddAsync(BlogPost blogPost);
        /// <summary>
/// Asynchronously updates an existing <see cref="BlogPost"/> in the data store.
/// </summary>
/// <param name="blogPost">The <see cref="BlogPost"/> containing updated values; should reference an existing post to be updated.</param>
Task UpdateAsync(BlogPost blogPost);
        /// <summary>
/// Asynchronously deletes the specified <see cref="BlogPost"/> from the data store.
/// </summary>
/// <param name="blogPost">The blog post entity to remove.</param>
Task DeleteAsync(BlogPost blogPost);
    }
}
