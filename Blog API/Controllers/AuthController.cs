using Blog_API.Models.Entities;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register register)
        {
            _logger.LogInformation("Attempting to register a new user.");
            if (register == null)
            {
                _logger.LogWarning("Registration attempt with null data.");
                return BadRequest("Invalid registration data.");
            }
            try
            {
                var result = await _authService.RegisterAsync(register);
                if (result) 
                
                {
                    _logger.LogInformation($"User registration successful. User Name: {register.UserName}");
                    return Ok("Registration successful.");
                }
                else
                {
                    _logger.LogWarning("User registration failed. Invalid data or user already exists.");
                    return BadRequest("Registration failed. Please check your data or if the user already exists.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                _logger.LogError(ex, "An error occurred during user registration.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login login)
        {
            _logger.LogInformation("Attempting to log in a user.");
            if (login == null)
            {
                _logger.LogWarning("Login attempt with null data.");
                return BadRequest("Invalid login data.");
            }
            try
            {
                var result = await _authService.LoginAsync(login);
                if (!result)
                {
                    _logger.LogWarning("Login failed. Invalid credentials.");
                    return Unauthorized("Login failed. Please check your credentials.");
                }
                _logger.LogInformation($"User login successful. User Name: {login.UserName}");
                return Ok("Login successful.");
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                _logger.LogError(ex, "An error occurred during user login.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Attempting to log out a user.");
            try
            {
                await _authService.LogoutAsync();
                _logger.LogInformation("User logout successful.");
                return Ok("Logout successful.");
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                _logger.LogError(ex, "An error occurred during user login.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
