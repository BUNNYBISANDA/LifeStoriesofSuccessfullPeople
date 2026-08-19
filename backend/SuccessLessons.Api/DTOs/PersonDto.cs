namespace SuccessLessons.Api.DTOs;

public record PersonDto(
    string Id,
    string Name,
    string Slug,
    string Era,
    string Field,
    string Summary,
    string ImageUrl,
    int FailureCount,
    List<string> Tags);

public record PersonDetailDto(
    PersonDto Person,
    List<ChapterSummaryDto> Chapters);
