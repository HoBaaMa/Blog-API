using Blog_API.Data;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Repositories.Implementations
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogDbContext _context;
        public BlogPostRepository(BlogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(BlogPost blogPost)
        {
            await _context.BlogPosts.AddAsync(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task<(IReadOnlyCollection<BlogPost> blogPosts, int totalCount)> GetAllAsync(PaginationRequest paginationRequest, string? filterOn, string? filterQuery, string? sortBy, bool? isAscending = true)
        {
            var query = _context.BlogPosts
                .AsNoTracking()
                .AsQueryable();

            // Filtering

            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(q => q.Title.Contains(filterQuery));
                }
            }

            // Sorting

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending ?? true ? query.OrderBy(q => q.CreatedAt) : query.OrderByDescending(q => q.CreatedAt);
                }
            }

            // Pagination
            var totalCount = await query.CountAsync();

            var BlogPosts = await query
                .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
                .Take(paginationRequest.PageSize)
                .Include(bp => bp.User)
                .ToListAsync();

            return (BlogPosts, totalCount);
        }

        public async Task<(IReadOnlyCollection<BlogPost> blogPosts, int totalCount)> GetBlogPostsByCategoryAsync(BlogCategory blogCategory, PaginationRequest paginationRequest)
        {
            var query = _context.BlogPosts
                .Where(bp => bp.BlogCategory == blogCategory)
                .AsNoTracking();

            // Get total count before applying pagination
            var totalCount = await query.CountAsync();

            // Apply pagination and include related data
            var blogPosts = await query
                .OrderByDescending(bp => bp.CreatedAt) // Order by creation date for consistent pagination
                .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
                .Take(paginationRequest.PageSize)
                .Include(bp => bp.User)
                .ToListAsync();

            return (blogPosts, totalCount);
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id) =>
            await _context.BlogPosts
                .Include(bp => bp.User)
                .Include(bp => bp.Likes)
                .Include(bp => bp.Tags) // Include tags for many-to-many relationship
                .Include(c => c.Comments.Where(c=> c.ParentCommentId == null))
                    .ThenInclude(c => c.Likes)
                    .ThenInclude(c => c.User)
                .Include(bp => bp.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                    .ThenInclude(r => r.Likes)
                    .ThenInclude(r => r.User)
                .Include(bp => bp.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(bp => bp.Id == id);

        public async Task UpdateAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
        }
    }
}
