using Microsoft.AspNetCore.Mvc;
using SuccessLessons.Api.DTOs;
using SuccessLessons.Api.Services;

namespace SuccessLessons.Api.Controllers;

[ApiController]
[Route("api/people")]
public class PeopleController : ControllerBase
{
    private readonly PeopleService _peopleService;

    public PeopleController(PeopleService peopleService)
    {
        _peopleService = peopleService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PersonDto>>> GetAll([FromQuery] string? field, [FromQuery] string? tag)
    {
        var people = await _peopleService.GetAllAsync(field, tag);
        return Ok(people.Select(p => new PersonDto(
            p.Id, p.Name, p.Slug, p.Era, p.Field, p.Summary, p.ImageUrl, p.FailureCount, p.Tags)));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<PersonDetailDto>> GetBySlug(string slug)
    {
        var person = await _peopleService.GetBySlugAsync(slug);
        if (person is null) return NotFound();

        var chapters = await _peopleService.GetChaptersForPersonAsync(person.Id);

        var dto = new PersonDetailDto(
            new PersonDto(person.Id, person.Name, person.Slug, person.Era, person.Field, person.Summary, person.ImageUrl, person.FailureCount, person.Tags),
            chapters.Select(c => new ChapterSummaryDto(c.Id, c.Title, c.Slug, c.Order, c.EstimatedReadMinutes)).ToList());

        return Ok(dto);
    }
}
