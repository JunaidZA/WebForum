using WebForum.Application.DTOs;
using WebForum.Domain.Entities;

namespace WebForum.Application.Converters;

public class TagConverter
{
    public static TagDto MapToDto(Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name
        };
    }
}
