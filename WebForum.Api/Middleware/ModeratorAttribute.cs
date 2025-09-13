using Microsoft.AspNetCore.Authorization;

namespace WebForum.Api.Middleware;

public class ModeratorAttribute : AuthorizeAttribute
{
    public ModeratorAttribute()
    {
        Policy = "Moderator";
    }
}
