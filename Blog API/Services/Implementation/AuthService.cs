using Blog_API.Data;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;
using Blog_API.Services.Interface;
using Blog_API.Utilities;
using Microsoft.AspNetCore.Identity;

namespace Blog_API.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(BlogDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            JwtTokenGenerator jwtTokenGenerator, ILogger<AuthService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string?> LoginAsync(LoginDTO login)
        {
            if (login == null)
                throw new ArgumentNullException(nameof(login), "Login data cannot be null");

            _logger.LogInformation("Attempting to authenticate user: {UserName}", login.UserName);

            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for username: {UserName}", login.UserName);
                return null;
            }

            _logger.LogDebug("User found: {UserId}, checking password", user.Id);

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, login.Password);
            if (!isPasswordCorrect)
            {
                _logger.LogWarning("Login failed: Invalid password for user: {UserName} (ID: {UserId})", login.UserName, user.Id);
                return null;
            }

            _logger.LogDebug("Password verified successfully for user: {UserName} (ID: {UserId})", login.UserName, user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || !roles.Any())
            {
                _logger.LogWarning("Login failed: No roles found for user: {UserName} (ID: {UserId})", login.UserName, user.Id);
                throw new InvalidOperationException($"User {login.UserName} has no assigned roles");
            }

            _logger.LogDebug("Retrieved roles for user {UserName}: {Roles}", login.UserName, string.Join(", ", roles));

            var token = _jwtTokenGenerator.CreateJwtToken(user, roles.ToList());
            _logger.LogInformation("Login successful for user: {UserName} (ID: {UserId}) with roles: {Roles}", 
                login.UserName, user.Id, string.Join(", ", roles));

            return token;
        }

        public async Task LogoutAsync()
        {
            _logger.LogInformation("Attempting to sign out user");
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User signed out successfully");
        }

        public async Task<bool> RegisterAsync(RegisterDTO register)
        {
            if (register == null)
                throw new ArgumentNullException(nameof(register), "Registration data cannot be null");

            _logger.LogInformation("Attempting to register new user: {UserName} with email: {Email}", register.UserName, register.Email);

            // Check if user already exists
            var existingUserByName = await _userManager.FindByNameAsync(register.UserName);
            if (existingUserByName != null)
            {
                _logger.LogWarning("Registration failed: Username {UserName} already exists", register.UserName);
                throw new InvalidOperationException($"Username '{register.UserName}' is already taken");
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(register.Email);
            if (existingUserByEmail != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", register.Email);
                throw new InvalidOperationException($"Email '{register.Email}' is already registered");
            }

            var user = new ApplicationUser
            {
                UserName = register.UserName,
                Email = register.Email
            };

            _logger.LogDebug("Creating user account for: {UserName}", register.UserName);

            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User creation failed for {UserName}: {Errors}", register.UserName, errors);
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            _logger.LogInformation("User account created successfully for: {UserName} (ID: {UserId})", register.UserName, user.Id);

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");
            _logger.LogInformation("Default 'User' role assigned to: {UserName} (ID: {UserId})", register.UserName, user.Id);

            _logger.LogInformation("Registration completed successfully for user: {UserName} (ID: {UserId})", register.UserName, user.Id);
            return true;
        }
    }
}
