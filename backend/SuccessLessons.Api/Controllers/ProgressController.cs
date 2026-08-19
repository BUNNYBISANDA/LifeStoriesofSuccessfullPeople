using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuccessLessons.Api.DTOs;
using SuccessLessons.Api.Services;

namespace SuccessLessons.Api.Controllers;

[ApiController]
[Route("api/progress")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly ProgressService _progressService;

    public ProgressController(ProgressService progressService)
    {
        _progressService = progressService;
    }

    private string Uid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<List<ProgressDto>>> GetAll()
    {
        var progress = await _progressService.GetForUserAsync(Uid);
        return Ok(progress.Select(ToDto));
    }

    [HttpGet("{chapterId}")]
    public async Task<ActionResult<ProgressDto>> GetForChapter(string chapterId)
    {
        var progress = await _progressService.GetForChapterAsync(Uid, chapterId);
        return progress is null ? NotFound() : Ok(ToDto(progress));
    }

    [HttpPut("{chapterId}")]
    public async Task<ActionResult<ProgressDto>> Upsert(string chapterId, UpsertProgressRequest request)
    {
        var progress = await _progressService.UpsertAsync(Uid, chapterId, request.PercentComplete, request.LastPositionBlockIndex);
        return Ok(ToDto(progress));
    }

    private static ProgressDto ToDto(Models.ReadingProgress p) =>
        new(p.ChapterId, p.PercentComplete, p.LastPositionBlockIndex, p.Completed, p.UpdatedAt.ToDateTime());
}
