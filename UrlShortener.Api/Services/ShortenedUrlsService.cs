using Dapper;
using Microsoft.Data.SqlClient;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Services;

public class ShortenedUrlsService : IShortenedUrlsService
{
    private const string PossibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int PathLength = 8;
    
    private readonly HashSet<string> reservedPaths = new() { "swagger", "create" };
    private readonly string dbConnectionString;

    private Random random = new Random();

    public ShortenedUrlsService(IConfiguration configuration)
    {
        dbConnectionString = configuration.GetConnectionString("UrlShortenerDatabase");
        
        if (string.IsNullOrWhiteSpace(dbConnectionString))
        {
            throw new ArgumentException("UrlShortenerDatabase connection string is null or white space");
        }
    }

    public async Task<IEnumerable<ShortenedUrl>> Get()
    {
        var query = "SELECT Id, OriginalUrl, GeneratedPath, CreatedAt FROM ShortenedUrls";
        
        using var dbConnection = new SqlConnection(dbConnectionString);
        var shortenedUrls = await dbConnection.QueryAsync<ShortenedUrl>(query);

        return shortenedUrls;
    }

    public async Task<ShortenedUrl?> GetByGeneratedPath(string path)
    {
        var query = "SELECT Id, OriginalUrl, GeneratedPath, CreatedAt FROM ShortenedUrls WHERE GeneratedPath = @path";
        
        using var dbConnection = new SqlConnection(dbConnectionString);
        var shortenedUrl = await dbConnection.QuerySingleOrDefaultAsync<ShortenedUrl>(query, new { path });
        
        return shortenedUrl;
    }

    public async Task<ShortenedUrl> Create(ShortenUrlRequest request)
    {
        var getQuery = "SELECT Id, OriginalUrl, GeneratedPath, CreatedAt FROM ShortenedUrls WHERE GeneratedPath = @generatedPath";
        var insertQuery = "INSERT INTO ShortenedUrls (OriginalUrl, GeneratedPath) VALUES (@url, @generatedPath)";
        
        using var dbConnection = new SqlConnection(dbConnectionString);
        string? generatedPath;

        while (true)
        {
            generatedPath = GeneratePath();
            
            if (reservedPaths.Contains(generatedPath))
            {
                continue;
            }
            
            var existingShortenedUrl = await dbConnection.QuerySingleOrDefaultAsync<ShortenedUrl>(getQuery, new { generatedPath });
            
            if (existingShortenedUrl is null)
            {
                break;
            }
        }

        await dbConnection.ExecuteAsync(insertQuery, new { request.Url, generatedPath });
        var shortenedUrl = await dbConnection.QuerySingleOrDefaultAsync<ShortenedUrl>(getQuery, new { generatedPath });

        return shortenedUrl;
    }

    public async Task Delete(int id)
    {
        var query = "DELETE FROM ShortenedUrls WHERE Id=@id";
        using var dbConnection = new SqlConnection(dbConnectionString);
        await dbConnection.ExecuteAsync(query, new { id });
    }

    private string GeneratePath()
    {
        var generatedChars = new char[PathLength];

        for (int i = 0; i < PathLength; i++)
        {
            int randomIndex = random.Next(PossibleChars.Length);
            generatedChars[i] = PossibleChars[randomIndex];
        }

        return new string(generatedChars);
    }
}
