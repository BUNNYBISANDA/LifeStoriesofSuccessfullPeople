using Microsoft.AspNetCore.Mvc;
using SuccessLessons.Api.DTOs;
using SuccessLessons.Api.Services;

namespace SuccessLessons.Api.Controllers;

[ApiController]
[Route("api/lessons")]
public class LessonsController : ControllerBase
{
    private readonly LessonsService _lessonsService;

    public LessonsController(LessonsService lessonsService)
    {
        _lessonsService = lessonsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LessonDto>>> GetAll([FromQuery] string? category)
    {
        var lessons = await _lessonsService.GetAllAsync(category);
        return Ok(lessons.Select(l => new LessonDto(l.Id, l.PersonId, l.ChapterId, l.Text, l.Category, l.IsFeatured)));
    }

    [HttpGet("featured")]
    public async Task<ActionResult<List<LessonDto>>> GetFeatured()
    {
        var lessons = await _lessonsService.GetFeaturedAsync();
        return Ok(lessons.Select(l => new LessonDto(l.Id, l.PersonId, l.ChapterId, l.Text, l.Category, l.IsFeatured)));
    }
}
