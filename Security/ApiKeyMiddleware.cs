using System.Net;

namespace UserService.Security;

/* Middleware som kontrollerar att skrivande API-anrop innehåller en giltig API-nyckel.
   Detta används för att skydda endpoints som skapar, uppdaterar eller raderar data. */
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

        // Tillåt login utan API-nyckel så att användare kan logga in
        if (path == "/api/users/login")
        {
            await _next(context);
            return;
        }

        // Skydda endast skrivande endpoints (POST, PUT, DELETE)
        var method = context.Request.Method;
        var isWriteMethod = method == HttpMethods.Post ||
                            method == HttpMethods.Put ||
                            method == HttpMethods.Delete;

        // GET-anrop tillåts utan API-nyckel
        if (!isWriteMethod)
        {
            await _next(context);
            return;
        }

        var expectedKey = config["ApiKey"];

        // Om API-nyckeln inte är konfigurerad i appsettings
        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("API key is not configured.");
            return;
        }

        // Kontrollera att requesten innehåller korrekt API-nyckel
        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey) ||
            providedKey != expectedKey)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Missing or invalid API key.");
            return;
        }

        // Om API-nyckeln är korrekt fortsätter requesten vidare i pipelinen
        await _next(context);
    }
}