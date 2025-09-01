using Blog_API.Attributes;
using Blog_API.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Blog_API.Models.DTOs
{
    public class CreateBlogPostDTO
    {
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [MinLength(10, ErrorMessage = "{0} must be at least {1} characters.")]
        public required string Content { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Range(0, 13,  ErrorMessage ="Invalid input, please choose a number between {1} and {2}.")]
        public BlogCategory BlogCategory { get; set; }
        
        [MaxLength(5, ErrorMessage = "Max tags per blog post is {1}.")]
        public ICollection<string> Tags { get; set; } = new List<string>();
        
        [MaxLength(8, ErrorMessage = "Maximum {1} images allowed per blog post.")]
        [ValidImageUrls(ErrorMessage = "One or more image URLs are invalid.")]
        public ICollection<string> ImageUrls { get; set; } = new List<string>();
    }
}
