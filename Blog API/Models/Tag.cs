namespace Blog_API.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public ICollection<BlogPost>? BlogPosts { get; set; } 
    }
}
