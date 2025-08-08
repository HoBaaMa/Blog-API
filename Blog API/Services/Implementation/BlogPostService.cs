using Blog_API.Models;
using Blog_API.Services.Interface;

namespace Blog_API.Services.Implementation
{
    public class BlogPostService : IBlogPostService
    {
        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
