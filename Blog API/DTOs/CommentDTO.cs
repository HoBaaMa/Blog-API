namespace Blog_API.DTOs
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int LikeCount { get; set; }
        public Guid? ParentCommentId { get; set; }
        public ICollection<CommentDTO> Replies { get; set; } = new List<CommentDTO>();
    }
}
