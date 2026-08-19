namespace SuccessLessons.Api.DTOs;

public record LessonDto(
    string Id,
    string PersonId,
    string ChapterId,
    string Text,
    string Category,
    bool IsFeatured);
