namespace WebForum.Api.Middleware;

public class JwtAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context).ConfigureAwait(false);
    }
}