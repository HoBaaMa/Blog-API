using Blog_API.Models.Entities;

namespace Blog_API.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(Guid id);
        Task<IReadOnlyCollection<Comment>> GetAllForBlogPostAsync(Guid blogPostId);
        Task AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
        Task<bool> IsParentExistsAsync(Guid parentCommentId, Guid blogPostId);
    }
}
