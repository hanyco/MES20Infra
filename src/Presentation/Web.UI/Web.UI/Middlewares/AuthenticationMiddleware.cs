using Library.Web.Bases;

namespace Web.UI.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            if (context.Response.StatusCode == 401)
            {
                context.Response.Redirect("/login");
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            context.Response.Redirect("/login");
        }
    }
}

public static class AuthenticationMiddlewareApplicationBuilderExtension
{
    public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<AuthenticationMiddleware>();
}