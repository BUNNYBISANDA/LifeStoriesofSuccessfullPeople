using Microsoft.AspNetCore.Mvc;
using SuccessLessons.Api.DTOs;
using SuccessLessons.Api.Services;

namespace SuccessLessons.Api.Controllers;

[ApiController]
[Route("api/chapters")]
public class ChaptersController : ControllerBase
{
    private readonly ChaptersService _chaptersService;

    public ChaptersController(ChaptersService chaptersService)
    {
        _chaptersService = chaptersService;
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ChapterDetailDto>> GetBySlug(string slug)
    {
        var chapter = await _chaptersService.GetBySlugAsync(slug);
        if (chapter is null) return NotFound();

        return Ok(new ChapterDetailDto(
            chapter.Id, chapter.PersonId, chapter.Title, chapter.Slug,
            chapter.Order, chapter.ContentBlocks, chapter.EstimatedReadMinutes));
    }
}
