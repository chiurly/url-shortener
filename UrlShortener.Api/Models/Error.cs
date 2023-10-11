using System.Net;
using System.Text.Json;

namespace UrlShortener.Api.Models;

public class Error
{
    public string ExceptionType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
