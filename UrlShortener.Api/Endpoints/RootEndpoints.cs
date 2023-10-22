using UrlShortener.Api.Services;

namespace UrlShortener.Api.Endpoints;

public static class RootEndpoints
{
    public static void MapRootEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/{path}", RedirectToOriginalUrl);
    }
    
    public static async Task<IResult> RedirectToOriginalUrl(string path, IShortenedUrlsService shortenedUrlService)
    {
        if (path is null)
        {
            return Results.BadRequest("Bad path");
        }

        var shortenedUrl = await shortenedUrlService.GetByGeneratedPath(path);

        if (shortenedUrl is null)
        {
            return Results.NotFound("Path not found");
        }

        return Results.Redirect(shortenedUrl.OriginalUrl);
    }
}
