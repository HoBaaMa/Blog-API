using Blog_API.Configurations;
using Blog_API.Models.DTOs;
using Blog_API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog_API.Controllers
{
    /// <summary>
    /// Controller for user authentication operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [EnableRateLimiting(RateLimitingConfiguration.AuthPolicy)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="register">Registration details including username, email, and password.</param>
        /// <returns>Success message with username on successful registration.</returns>
        [HttpPost("register")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            _logger.LogInformation("API registration request received for username: {UserName}", register?.UserName ?? "null");
            
            var result = await _authService.RegisterAsync(register!);
            if (result)
            {
                _logger.LogInformation("User registration successful via API for username: {UserName}", register!.UserName);
                return Ok(new { Message = "Registration successful. User has been assigned User role.", UserName = register.UserName });
            }
            
            _logger.LogWarning("Registration failed via API for username: {UserName} - unexpected false result", register!.UserName);
            return BadRequest(new { Message = "Registration failed for unknown reason" });
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="login">Login credentials including username and password.</param>
        /// <returns>JWT token on successful authentication.</returns>
        [HttpPost("login")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            _logger.LogInformation("API login request received for username: {UserName}", login?.UserName ?? "null");
            
            var token = await _authService.LoginAsync(login!);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Login failed via API for username: {UserName} - invalid credentials", login!.UserName);
                return Unauthorized(new { Message = "Login failed. Please check your credentials." });
            }
            
            _logger.LogInformation("User login successful via API for username: {UserName}", login!.UserName);
            return Ok(new { Token = token, Message = "Login successful", UserName = login.UserName });
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>Success message on logout.</returns>
        [HttpPost("logout")]
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name ?? "Unknown";
            _logger.LogInformation("API logout request received for user: {UserName}", userName);
            
            await _authService.LogoutAsync();
            _logger.LogInformation("User logout successful via API for user: {UserName}", userName);
            return Ok(new { Message = "Logout successful", UserName = userName });
        }
    }
}

