using Blog_API.Models.DTOs;

namespace Blog_API.Services.Interface
{
    public interface ILikeService
    {
        Task<bool> ToggleLikeAsync(string userId, Guid? blogPostId = null, Guid? commentId = null);
        Task<IReadOnlyCollection<LikeDTO>> GetAllLikesByBlogPostIdAsync(Guid blogPostId);
        Task<IReadOnlyCollection<LikeDTO>> GetAllLikesByCommentIdAsync(Guid commentId);
    }
}
