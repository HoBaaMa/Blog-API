using Blog_API.Models.DTOs;

namespace Blog_API.Services.Interface
{
    public interface IAuthService
    {
        //string GetCurrentUserId();
        Task<bool> RegisterAsync(RegisterDTO register);
        Task<string?> LoginAsync(LoginDTO login);
        Task LogoutAsync();
    }
}
