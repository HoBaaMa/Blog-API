using AutoMapper;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Blog_API.Services.Interface;

namespace Blog_API.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<BlogCategoryDTO> CreateBlogCategoryAsync(CreateBlogCategoryDTO blogCategoryDTO)
        {
            if (await _categoryRepository.IsCategoryExistsAsync(blogCategoryDTO.Name))
            {
                throw new ArgumentException(nameof(blogCategoryDTO.Name), "Already exists.");
            };
            var category = _mapper.Map<BlogCategory>(blogCategoryDTO);
            await _categoryRepository.AddAsync(category);
            return _mapper.Map<BlogCategoryDTO>(category);
        }

        public async Task DeleteBlogCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetBlogCategoryByIdAsync(id);

            if (category is null)
            {
                throw new KeyNotFoundException($"Blog Category ID: {id} not found.");
            }

            await _categoryRepository.DeleteAsync(category);
        }

        public async Task<IReadOnlyCollection<BlogCategoryDTO>> GetAllBlogCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return _mapper.Map<IReadOnlyCollection<BlogCategoryDTO>>(categories);
        }

        public async Task<bool> IsCategoryExistsAsync(string name) => await _categoryRepository.IsCategoryExistsAsync(name);
        
    }
}
