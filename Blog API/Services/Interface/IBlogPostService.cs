using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

namespace Blog_API.Services.Interface
{
    public interface IBlogPostService
    {
        Task<IReadOnlyCollection<BlogPostDTO>> GetAllBlogPostsAsync();
        Task<PagedResult<BlogPostDTO>> GetBlogPostsByCategoryAsync(BlogCategory blogCategory, PaginationRequest paginationRequest);
        Task<BlogPostDTO?> GetBlogPostByIdAsync(Guid id);
        Task<BlogPostDTO> CreateBlogPostAsync(CreateBlogPostDTO blogPostDTO, string userId);
        Task<BlogPostDTO> UpdateBlogPostAsync(Guid id, CreateBlogPostDTO blogPostDTO, string currentUserId);
        Task DeleteBlogPostAsync(Guid id, string currentUserId);
        Task<IReadOnlyCollection<string>> GetBlogPostImagesAsync(Guid blogPostId);
    }
}
