namespace Blog_API.Models.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public Guid BlogPostId { get; set; }
        public BlogPost? BlogPost { get; set; }
        public string UserId { get; set; } = default!;
        public ApplicationUser? User { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public Guid? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    }
}