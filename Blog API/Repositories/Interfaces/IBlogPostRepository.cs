using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

namespace Blog_API.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<BlogPost?> GetByIdAsync(Guid id);
        Task<IReadOnlyCollection<BlogPost>> GetAllAsync();
        Task<IReadOnlyCollection<BlogPost>> GetBlogPostsByCategoryAsync(BlogCategory blogCategory);
        Task<(IReadOnlyCollection<BlogPost> blogPosts, int totalCount)> GetBlogPostsByCategoryPagedAsync(BlogCategory blogCategory, PaginationRequest paginationRequest);
        Task AddAsync(BlogPost blogPost);
        Task UpdateAsync(BlogPost blogPost);
        Task DeleteAsync(BlogPost blogPost);
    }
}
