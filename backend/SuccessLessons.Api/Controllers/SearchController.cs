using Microsoft.AspNetCore.Mvc;
using SuccessLessons.Api.DTOs;
using SuccessLessons.Api.Services;

namespace SuccessLessons.Api.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly PeopleService _peopleService;
    private readonly LessonsService _lessonsService;

    public SearchController(PeopleService peopleService, LessonsService lessonsService)
    {
        _peopleService = peopleService;
        _lessonsService = lessonsService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return Ok(new { people = Array.Empty<PersonDto>(), lessons = Array.Empty<LessonDto>() });

        var needle = q.Trim().ToLowerInvariant();

        var people = await _peopleService.GetAllAsync();
        var matchedPeople = people
            .Where(p => p.Name.ToLowerInvariant().Contains(needle) || p.Summary.ToLowerInvariant().Contains(needle))
            .Select(p => new PersonDto(p.Id, p.Name, p.Slug, p.Era, p.Field, p.Summary, p.ImageUrl, p.FailureCount, p.Tags));

        var lessons = await _lessonsService.GetAllAsync();
        var matchedLessons = lessons
            .Where(l => l.Text.ToLowerInvariant().Contains(needle))
            .Select(l => new LessonDto(l.Id, l.PersonId, l.ChapterId, l.Text, l.Category, l.IsFeatured));

        return Ok(new { people = matchedPeople, lessons = matchedLessons });
    }
}
