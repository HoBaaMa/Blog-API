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
        // TODO : Custom Validation on blog category
        public BlogCategory BlogCategory { get; set; }
        public ICollection<string> Tags { get; set; } = new List<string>();
    }
}
