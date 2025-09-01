using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

namespace Blog_API.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<BlogPost?> GetByIdAsync(Guid id);
        Task<(IReadOnlyCollection<BlogPost> blogPosts, int totalCount)> GetAllAsync(PaginationRequest paginationRequest, string? filterOn, string? filterQuery, string? sortBy, bool? isAscending = true);
        Task<(IReadOnlyCollection<BlogPost> blogPosts, int totalCount)> GetBlogPostsByCategoryAsync(BlogCategory blogCategory, PaginationRequest paginationRequest);
        Task AddAsync(BlogPost blogPost);
        Task UpdateAsync(BlogPost blogPost);
        Task DeleteAsync(BlogPost blogPost);
    }
}
