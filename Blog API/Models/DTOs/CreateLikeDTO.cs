using System.ComponentModel.DataAnnotations;

namespace Blog_API.Models.DTOs
{
    public class CreateLikeDTO
    {
        // TODO : Custome Validation
        public Guid? BlogPostId { get; set; }
        public Guid? CommentId { get; set; }
    }
}
