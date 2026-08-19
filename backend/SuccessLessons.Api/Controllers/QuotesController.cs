using Microsoft.AspNetCore.Mvc;
using SuccessLessons.Api.Services;

namespace SuccessLessons.Api.Controllers;

[ApiController]
[Route("api/quotes")]
public class QuotesController : ControllerBase
{
    private readonly LessonsService _lessonsService;

    public QuotesController(LessonsService lessonsService)
    {
        _lessonsService = lessonsService;
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandom()
    {
        var quote = await _lessonsService.GetRandomQuoteAsync();
        return quote is null ? NotFound() : Ok(quote);
    }
}
