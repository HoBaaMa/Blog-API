using Blog_API.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Blog_API.DTOs
{
    public class BlogPostDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public int LikeCount { get; set; }
        public ICollection<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
        public BlogCategory BlogCategory { get; set; }
        public ICollection<TagDTO> Tags { get; set; } = new List<TagDTO>();
    }
}
