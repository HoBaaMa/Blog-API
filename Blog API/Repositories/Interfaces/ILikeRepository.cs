using Blog_API.Models.Entities;

namespace Blog_API.Repositories.Interfaces
{
    public interface ILikeRepository
    {
        Task<IReadOnlyCollection<Like>> GetLikesByBlogPostIdAsync(Guid blogPostId);
        Task<IReadOnlyCollection<Like>> GetLikesByCommentIdAsync(Guid commentId);
        Task AddAsync(Like like);
        Task RemoveAsync(Like like);
        Task<Like?> GetByUserAndTargetAsync(string userId, Guid? blogPostId, Guid? commentId);

    }
}
