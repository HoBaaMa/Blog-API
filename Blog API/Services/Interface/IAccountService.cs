using Blog_API.Models.Entities;

namespace Blog_API.Services.Interface
{
    public interface IAccountService
    {
        public Task<bool> RegisterAsync(Register register);
        public Task<bool> LoginAsync(Login login);
        public Task LogoutAsync();
    }
}
