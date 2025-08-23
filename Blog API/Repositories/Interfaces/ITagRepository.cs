using Blog_API.Models.Entities;

namespace Blog_API.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag?> GetTagByNameAsync(string name);

        Task AddAsync(Tag tag);
    }
}
