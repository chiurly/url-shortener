using UrlShortener.Api.Endpoints;
using UrlShortener.Api.Middlewares;
using UrlShortener.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IShortenedUrlsService, ShortenedUrlsService>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapShortenedUrlsEndpoints();
app.MapGet("/{path}", RedirectToOriginalUrl);

app.Run();

async Task<IResult> RedirectToOriginalUrl(string path, IShortenedUrlsService shortenedUrlService)
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
