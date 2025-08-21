using Blog_API.DTOs;

namespace Blog_API.Services.Interface
{
    public interface IBlogPostService
    {
        Task<IReadOnlyCollection<BlogPostDTO>> GetAllBlogPostsAsync();
        Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id);
        Task<BlogPostDTO> CreateBlogPostAsync(CreateBlogPostDTO blogPostDTO, string userId);
        Task<BlogPostDTO> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO, string currentUserId);
        Task DeleteBlogPostAsync(Guid id, string currentUserId);
    }
}
