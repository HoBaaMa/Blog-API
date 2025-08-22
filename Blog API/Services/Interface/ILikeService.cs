using Blog_API.Models.DTOs;

namespace Blog_API.Services.Interface
{
    public interface ILikeService
    {
        Task<bool> ToggleLikeAsync(CreateLikeDTO likeDTO, string userId);
    }
}
