using Blog_API.Data;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Repositories.Implementations
{
    public class TagRepository : ITagRepository
    {
        private readonly BlogDbContext _context;
        public TagRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Tag tag)
        {
            tag.Name = tag.Name.Trim().ToUpperInvariant();

            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
        }
    }
}
