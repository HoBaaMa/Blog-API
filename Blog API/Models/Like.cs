namespace Blog_API.Models
{
    public class Like
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Guid? BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }
        public Guid? CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}
