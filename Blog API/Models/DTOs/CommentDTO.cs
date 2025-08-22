namespace Blog_API.Models.DTOs
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public int LikeCount { get; set; }
        public Guid? ParentCommentId { get; set; }
        public ICollection<CommentDTO> Replies { get; set; } = new List<CommentDTO>();
    }
}
