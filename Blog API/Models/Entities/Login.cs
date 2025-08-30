using System.ComponentModel.DataAnnotations;

namespace Blog_API.Models.Entities
{
    public class Login
    {
        [Required(ErrorMessage = "{0} is required.")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} is required.")]
        public string Password { get; set; } = string.Empty;

        // You can add additional properties if needed, such as:
        public bool RememberMe { get; set; } // For "Remember Me" functionality
        // public string ReturnUrl { get; set; } // For redirecting after login
    }
}
