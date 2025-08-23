using Blog_API.Models.Entities;
namespace Blog_API.Models.DTOs
{
    public class LikeDTO
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
