using System.ComponentModel.DataAnnotations;

namespace Blog_API.Models.Entities
{
    public class BlogPost
    {
        public Guid Id { get; set; }
        public required string Title { get; set; } 
        public required string Content { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserId { get; set; } = default!;
        public ApplicationUser? User { get; set; } 
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public BlogCategory BlogCategory { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<string> ImageUrls { get; set; } = new List<string>();
        //public string Slug { get; set; } = default!;
    }
}
