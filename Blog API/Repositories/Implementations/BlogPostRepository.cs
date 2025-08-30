using Blog_API.Data;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;
using Blog_API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Removes the given BlogPost entity from the database and persists the change.
        /// </summary>
        /// <param name="blogPost">The BlogPost entity to remove.</param>
        public async Task DeleteAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all blog posts with related data, optionally filtered and sorted.
        /// </summary>
        /// <remarks>
        /// The query is executed with no tracking and eagerly loads User, Likes, Tags, and top-level Comments (with their Users, Likes and Replies).
        /// Supported filters: when <paramref name="filterOn"/> equals "Title" (case-insensitive), posts whose Title contains <paramref name="filterQuery"/> are returned.
        /// Supported sorting: when <paramref name="sortBy"/> equals "CreatedAt" (case-insensitive), results are ordered by the CreatedAt timestamp.
        /// </remarks>
        /// <param name="filterOn">The field to filter on (currently supports "Title"); ignored if null or whitespace.</param>
        /// <param name="filterQuery">The substring to search for when filtering; ignored if null or whitespace.</param>
        /// <param name="sortBy">The field to sort by (currently supports "CreatedAt"); ignored if null or whitespace.</param>
        /// <param name="isAscending">When sorting, true for ascending order and false for descending. Defaults to true when null.</param>
        /// <returns>A read-only collection of matching <see cref="BlogPost"/> entities including their related data.</returns>
        public async Task<IReadOnlyCollection<BlogPost>> GetAllAsync(string? filterOn, string? filterQuery, string? sortBy, bool? isAscending = true)
        {
            var query = _context.BlogPosts
                .AsNoTracking()
                .Include(bp => bp.User)
                .Include(bp => bp.Likes)
                .Include(bp => bp.Tags) // Include tags for many-to-many relationship
                .Include(c => c.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.User)
                .Include(bp => bp.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Likes)
                    .ThenInclude(l => l.User)
                .Include(c => c.Comments
                    .Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                    .ThenInclude(u => u.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(q => q.Title.Contains(filterQuery));
                }
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending ?? true ? query.OrderBy(q => q.CreatedAt) : query.OrderByDescending(q => q.CreatedAt);
                }
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves a paged list of blog posts for the specified category along with the total number of posts in that category.
        /// </summary>
        /// <param name="blogCategory">The category to filter blog posts by.</param>
        /// <param name="paginationRequest">Pagination settings (PageNumber and PageSize) used to page the results.</param>
        /// <returns>
        /// A tuple where <c>blogPosts</c> is the requested page of blog posts (includes User, Tags, Likes and top-level Comments with their User)
        /// and <c>totalCount</c> is the total number of blog posts in the specified category before paging is applied.
        /// </returns>
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
                .Include(bp => bp.Tags)
                .Include(bp => bp.Likes)
                .Include(bp => bp.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.User)
                .ToListAsync();

            return (blogPosts, totalCount);
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id) =>
            await _context.BlogPosts
                .Include(bp => bp.User)
                .Include(bp => bp.Likes)
                .Include(bp => bp.Tags) // Include tags for many-to-many relationship
                .Include(c => c.Comments.Where(c=> c.ParentCommentId == null))
                    .ThenInclude(c => c.User)
                .Include(bp => bp.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Likes)
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
