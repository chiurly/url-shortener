using UrlShortener.Api.Models;

namespace UrlShortener.Api.Services;

public interface IShortenedUrlsService
{
    public Task<IEnumerable<ShortenedUrl>> Get();
    public Task<ShortenedUrl?> GetByGeneratedPath(string path);
    public Task<ShortenedUrl> Create(ShortenUrlRequest request);
    public Task Delete(int id);
}
