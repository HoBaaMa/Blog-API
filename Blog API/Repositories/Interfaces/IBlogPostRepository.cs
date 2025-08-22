using Blog_API.Models.Entities;

namespace Blog_API.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<BlogPost?> GetByIdAsync(Guid id);
        Task<IReadOnlyCollection<BlogPost>> GetAllAsync();
        Task AddAsync(BlogPost blogPost);
        Task UpdateAsync(BlogPost blogPost);
        Task DeleteAsync(BlogPost blogPost);
    }
}
