using Dapper;
using Microsoft.Data.SqlClient;
using URLShortener.API.Models;

namespace UrlShortener.Api.Services;

public class ShortenUrlService : IShortenUrlService
{
    private const string PossibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int PathLength = 8;
    
    private readonly HashSet<string> reservedPaths = new() { "create" };
    private readonly string dbConnectionString;

    private Random random = new Random();

    public ShortenUrlService(IConfiguration configuration)
    {
        dbConnectionString = configuration.GetConnectionString("UrlShortenerDatabase");
        
        if (string.IsNullOrWhiteSpace(dbConnectionString))
        {
            throw new ArgumentNullException(nameof(dbConnectionString));
        }
    }

    public async Task<ShortenedUrl?> GetByGeneratedPath(string path)
    {
        var getQuery = "SELECT Id, OriginalUrl, GeneratedPath, CreatedAt FROM ShortenedUrls WHERE GeneratedPath = @path";

        using var dbConnection = new SqlConnection(dbConnectionString);
        var shortenedUrl = await dbConnection.QuerySingleOrDefaultAsync<ShortenedUrl>(getQuery, new { path });

        return shortenedUrl;
    }

    public async Task<ShortenedUrl?> Create(ShortenUrlRequest request)
    {
        var insertQuery = "INSERT INTO ShortenedUrls (OriginalUrl, GeneratedPath) VALUES (@url, @generatedPath)";
        var getQuery = "SELECT Id, OriginalUrl, GeneratedPath, CreatedAt FROM ShortenedUrls WHERE GeneratedPath = @generatedPath";

        using var dbConnection = new SqlConnection(dbConnectionString);
        string? generatedPath;

        while (true)
        {
            generatedPath = GeneratePath();
            var existingShortenedUrl = await dbConnection.QuerySingleOrDefaultAsync<ShortenedUrl>(getQuery, new { generatedPath });

            if (!reservedPaths.Contains(generatedPath) && existingShortenedUrl is null)
            {
                break;
            }
        }

        await dbConnection.ExecuteAsync(insertQuery, new { request.Url, generatedPath });
        var shortenedUrl = await dbConnection.QuerySingleOrDefaultAsync<ShortenedUrl>(getQuery, new { generatedPath });

        return shortenedUrl;
    }

    private string GeneratePath()
    {
        var generatedChars = new char[PossibleChars.Length];

        for (int i = 0; i < PathLength; i++)
        {
            int randomIndex = random.Next(PossibleChars.Length);
            generatedChars[i] = PossibleChars[randomIndex];
        }

        return new string(generatedChars);
    }
}
