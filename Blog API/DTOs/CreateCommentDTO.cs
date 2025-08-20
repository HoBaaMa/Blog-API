using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_API.DTOs
{
    public class CreateCommentDTO
    {
        [DisplayName("Comment")]
        [Required(ErrorMessage = "{0} content is required.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "{0} must be between {2} and {1} characters.")]
        public required string Content { get; set; }
        public Guid BlogPostId { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}
