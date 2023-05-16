namespace Conduit;

public static class HttpContextExtensions
{
    public static string? GetUserEmail(this IHttpContextAccessor contextAccessor)
        => contextAccessor?.HttpContext?.Items["Email"]?.ToString();

    public static string? GetUserName(this IHttpContextAccessor contextAccessor)
        => contextAccessor?.HttpContext?.Items["UserName"]?.ToString();
}