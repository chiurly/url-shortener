using UrlShortener.Api.Services;
using UrlShortener.Api.Models;
using Azure.Core;

namespace UrlShortener.Api.Endpoints;

public static class ShortenedUrlsEndpoints
{
    public static void MapShortenedUrlsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/shortenedurls", Get);
        builder.MapPost("api/shortenedurls", Create);
        builder.MapDelete("api/shortenedurls", Delete);
    }

    public static async Task<IResult> Get(IShortenedUrlsService shortenedUrlsService)
    {
        var shortenedUrls = await shortenedUrlsService.Get();
        
        return Results.Ok(shortenedUrls);
    }

    public static async Task<IResult> Create(IShortenedUrlsService shortenedUrlsService, HttpContext httpContext, ShortenUrlRequest request)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
        {
            return Results.BadRequest("Bad URL");
        }

        var shortenedUrl = await shortenedUrlsService.Create(request);
        string baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

        return Results.Ok($"{baseUrl}/{shortenedUrl.GeneratedPath}");
    }

    public static async Task<IResult> Delete(IShortenedUrlsService shortenedUrlsService, int id)
    {
        await shortenedUrlsService.Delete(id);

        return Results.NoContent();
    }
}
