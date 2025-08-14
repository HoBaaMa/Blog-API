using Blog_API.DTOs;

namespace Blog_API.Services.Interface
{
    public interface IBlogPostService
    {
        public Task<IReadOnlyCollection<BlogPostDTO>> GetAllBlogPostsAsync();
        public Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id);
        public Task<BlogPostDTO> CreateBlogPostAsync(CreateBlogPostDTO blogPostDTO, string userId);
        public Task<BlogPostDTO> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO);
        public Task DeleteBlogPostAsync(Guid id, string currentUserId);
    }
}
