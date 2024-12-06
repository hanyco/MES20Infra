using System.Net;

using Blazored.LocalStorage;

namespace Web.UI.Middlewares;

public class AuthenticationMiddleware
{
    private readonly ILocalStorageService _localStorage;
    private readonly RequestDelegate _next;
    public AuthenticationMiddleware(RequestDelegate next, ILocalStorageService localStorage)
    {
        this._next = next;
        this._localStorage = localStorage;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this._next(context);
            if (context.Response.StatusCode == 401)
            {
                context.Response.Redirect("/login");
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
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