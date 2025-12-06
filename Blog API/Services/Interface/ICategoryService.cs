using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

namespace Blog_API.Services.Interface
{
    public interface ICategoryService 
    {
        Task<BlogCategoryDTO> CreateBlogCategoryAsync(CreateBlogCategoryDTO blogCategoryDTO);
        Task DeleteBlogCategoryAsync(Guid id);
        Task<IReadOnlyCollection<BlogCategoryDTO>> GetAllBlogCategoriesAsync();
        Task<bool> IsCategoryExistsAsync(string name);

    }
}
