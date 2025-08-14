using Blog_API.DTOs;
using Blog_API.Models;

namespace Blog_API.Services.Interface
{
    public interface IBlogPostService
    {
        public Task<IEnumerable<BlogPostDTO>> GetAllBlogPostsAsync();
        public Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id);
        public Task<BlogPostDTO?> CreateBlogPostAsync(CreateBlogPostDTO blogPostDTO, string userId);
        public Task<BlogPost?> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO);
        public Task DeleteBlogPostAsync(Guid id);
    }
}
