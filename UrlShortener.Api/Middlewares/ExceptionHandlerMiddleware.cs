using System.Net;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            var error = new Error
            {
                ExceptionType = exception.GetType().Name,
                Message = exception.Message,
                StatusCode = HttpStatusCode.InternalServerError,
                //StatusCode = exception switch
                //{
                //    ?Exception => HttpStatusCode.?
                //    _ => HttpStatusCode.InternalServerError
                //}
            };

            _logger.LogError(exception.ToString());
            await httpContext.Response.WriteAsJsonAsync(error);
        }
    }
}
