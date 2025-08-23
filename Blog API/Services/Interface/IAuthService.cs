using Blog_API.Models.Entities;

namespace Blog_API.Services.Interface
{
    public interface IAuthService
    {
        //string GetCurrentUserId();
        Task<bool> RegisterAsync(Register register);
        Task<bool> LoginAsync(Login login);
        Task LogoutAsync();
    }
}
