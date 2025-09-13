using WebForum.Domain.Entities;

namespace WebForum.Domain.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByNameAsync(string name);
    Task<Tag> CreateAsync(Tag tag);
}