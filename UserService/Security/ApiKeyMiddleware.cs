using System.Net;

namespace UserService.Security;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string HeaderName = "X-API-KEY";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration config)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();

        // Tillåt login utan API-nyckel
        if (path == "/api/users/login")
        {
            await _next(context);
            return;
        }

        // Skydda bara skrivande endpoints
        var method = context.Request.Method;
        var isWriteMethod = method == HttpMethods.Post ||
                            method == HttpMethods.Put ||
                            method == HttpMethods.Delete;

        if (!isWriteMethod)
        {
            await _next(context);
            return;
        }

        var expectedKey = config["ApiKey"];

        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("API key is not configured.");
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey) ||
            providedKey != expectedKey)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Missing or invalid API key.");
            return;
        }

        await _next(context);
    }
}