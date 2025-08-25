using Blog_API.Models.Entities;


namespace Blog_API.Models.DTOs
{
    public class BlogPostDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public int LikeCount { get; set; }
        public ICollection<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
        public BlogCategory BlogCategory { get; set; }
        public ICollection<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public ICollection<string> ImageUrls { get; set; } = new List<string>();
        //public string? Slug { get; set; }
    }
}
