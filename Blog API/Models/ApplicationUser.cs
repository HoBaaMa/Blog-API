using Microsoft.AspNetCore.Identity;

namespace Blog_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        // Additional properties can be added here if needed
        // For example, you might want to add a DisplayName or ProfilePictureUrl
        // public string DisplayName { get; set; }
        // public string ProfilePictureUrl { get; set; }
    }
}
