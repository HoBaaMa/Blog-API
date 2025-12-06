using Blog_API.Data;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BlogDbContext _context;

        public CategoryRepository(BlogDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(BlogCategory blogCategory)
        {
            BlogCategory? category = await _context.BlogCategories.FirstOrDefaultAsync(bc => bc.Name == blogCategory.Name.ToUpper());
            if (category is null)
            {
                blogCategory.Name = blogCategory.Name.ToUpper();
                await _context.BlogCategories.AddAsync(blogCategory);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(BlogCategory blogCategory)
        {
            _context.BlogCategories.Remove(blogCategory);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<BlogCategory>> GetAllAsync() => await _context.BlogCategories.ToListAsync();

        public async Task<BlogCategory?> GetBlogCategoryByIdAsync(Guid id) => await _context.BlogCategories.FirstOrDefaultAsync(bc => bc.Id == id);

        public async Task<bool> IsCategoryExistsAsync(string name) => await _context.BlogCategories.AnyAsync(bc => bc.Name == name);
        
    }
}
