using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuccessLessons.Api.DTOs;
using SuccessLessons.Api.Services;

namespace SuccessLessons.Api.Controllers;

[ApiController]
[Route("api/highlights")]
[Authorize]
public class HighlightsController : ControllerBase
{
    private readonly HighlightService _highlightService;

    public HighlightsController(HighlightService highlightService)
    {
        _highlightService = highlightService;
    }

    private string Uid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("{chapterId}")]
    public async Task<ActionResult<List<HighlightDto>>> GetForChapter(string chapterId)
    {
        var highlights = await _highlightService.GetForChapterAsync(Uid, chapterId);
        return Ok(highlights.Select(h => new HighlightDto(h.Id, h.ChapterId, h.BlockIndex, h.SelectedText, h.Note, h.CreatedAt.ToDateTime())));
    }

    [HttpPost]
    public async Task<ActionResult<HighlightDto>> Create(CreateHighlightRequest request)
    {
        var highlight = await _highlightService.AddAsync(Uid, request.ChapterId, request.BlockIndex, request.SelectedText, request.Note);
        return Ok(new HighlightDto(highlight.Id, highlight.ChapterId, highlight.BlockIndex, highlight.SelectedText, highlight.Note, highlight.CreatedAt.ToDateTime()));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _highlightService.DeleteAsync(Uid, id);
        return deleted ? NoContent() : NotFound();
    }
}
