using UrlShortener.Api.Endpoints;
using UrlShortener.Api.Middlewares;
using UrlShortener.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IShortenedUrlsService, ShortenedUrlsService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapRootEndpoints();
app.MapShortenedUrlsEndpoints();

app.Run();
