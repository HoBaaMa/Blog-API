using System.ComponentModel.DataAnnotations;

namespace Blog_API.DTOs
{
    public class CreateLikeDTO
    {
        public Guid? BlogPostId { get; set; }
        public Guid? CommentId { get; set; }
    }
}
