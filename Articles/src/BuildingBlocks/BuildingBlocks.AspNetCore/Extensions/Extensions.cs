using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.AspNetCore.Extensions;

public static class Extensions
{
    public static string? BaseUrl(this HttpRequest? request)
    {
        if (request is null)
            return null;

        var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host, request.Host.Port ?? -1);
        if (uriBuilder.Uri.IsDefaultPort)
            uriBuilder.Port = -1;
        
        return  uriBuilder.Uri.AbsoluteUri;
    }

    public static string GetClientIpAddress(this HttpContext httpContext)
    {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }
        
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}