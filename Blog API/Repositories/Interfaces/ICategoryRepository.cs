using Blog_API.Models.Entities;

namespace Blog_API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddAsync(BlogCategory blogCategory);
        Task DeleteAsync(BlogCategory blogCategory);
        Task<IReadOnlyCollection<BlogCategory>> GetAllAsync();
        Task<bool> IsCategoryExistsAsync(string name);
        Task<BlogCategory?> GetBlogCategoryByIdAsync(Guid id);
    }
}
