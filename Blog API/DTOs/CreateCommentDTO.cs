using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_API.DTOs
{
    public class CreateCommentDTO
    {
        [DisplayName("Comment")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "{0} length should be between {2} and {1}.")]
        public required string Content { get; set; }
        public Guid BlogPostId { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}
