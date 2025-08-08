using Blog_API.Models;

namespace Blog_API.Services.Interface
{
    public interface IBlogPostService
    {
        public Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync();
        public Task<BlogPost?> GetBlogPostByIdAsync(Guid id);
    }
}
