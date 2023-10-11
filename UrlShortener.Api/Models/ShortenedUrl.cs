namespace UrlShortener.Api.Models;

public class ShortenedUrl
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string GeneratedPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
