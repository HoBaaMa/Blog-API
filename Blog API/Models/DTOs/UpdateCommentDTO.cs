using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_API.Models.DTOs
{
    public class UpdateCommentDTO
    {
        [DisplayName("Comment")]
        [Required(ErrorMessage = "{0} content is required.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "{0} must be between {2} and {1} characters.")]
        public required string Content { get; set; }
    }
}
