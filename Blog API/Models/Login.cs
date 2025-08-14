namespace Blog_API.Models
{
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        // You can add additional properties if needed, such as:
         public bool RememberMe { get; set; } // For "Remember Me" functionality
        // public string ReturnUrl { get; set; } // For redirecting after login
    }
}
