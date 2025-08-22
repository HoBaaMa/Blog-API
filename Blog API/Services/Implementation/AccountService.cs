using Blog_API.Data;
using Blog_API.Models.Entities;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace Blog_API.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountService(BlogDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> LoginAsync(Login login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password,  login.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return false;
            }
            return true;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> RegisterAsync(Register register)
        {
            var user = new ApplicationUser
            {
                UserName = register.UserName,
                Email = register.Email
            };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
            {
                // Handle errors, e.g., log them or throw an exception
                return false;
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return true;
        }
    }
}
