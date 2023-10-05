using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Services;
using URLShortener.API.Models;

namespace URLShortener.API.Controllers;

[ApiController]
public class ShortenUrlController : ControllerBase
{
    private readonly IShortenUrlService _shortenUrlService;

    public ShortenUrlController(IShortenUrlService shortenUrlService)
    {
        _shortenUrlService = shortenUrlService;
    }

    [HttpGet("{path}")]
    public async Task<IActionResult> Get(string path)
    {
        if (path is null)
        {
            return BadRequest("Bad path");
        }

        var shortenedUrl = await _shortenUrlService.GetByGeneratedPath(path);

        if (shortenedUrl is null)
        {
            return NotFound("Path not found");
        }

        return Redirect(shortenedUrl.OriginalUrl);
    }

    [HttpPost("api/create")]
    public async Task<IActionResult> Create(ShortenUrlRequest request)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
        {
            return BadRequest("Bad URL");
        }

        var shortenedUrl = await _shortenUrlService.Create(request);

        if (shortenedUrl is null)
        {
            return BadRequest();
        }

        return Ok($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{shortenedUrl.GeneratedPath}");
    }
}
