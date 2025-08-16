namespace Blog_API.DTOs
{
    public class BlogPostDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int LikeCount { get; set; }

        // Comments ICollection
    }
}
