using URLShortener.API.Models;

namespace UrlShortener.Api.Services;

public interface IShortenUrlService
{
    public Task<ShortenedUrl?> GetByGeneratedPath(string path);
    public Task<ShortenedUrl?> Create(ShortenUrlRequest request);
}
