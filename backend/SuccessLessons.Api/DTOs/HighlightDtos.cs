namespace SuccessLessons.Api.DTOs;

public record HighlightDto(
    string Id,
    string ChapterId,
    int BlockIndex,
    string SelectedText,
    string Note,
    DateTime CreatedAt);

public record CreateHighlightRequest(
    string ChapterId,
    int BlockIndex,
    string SelectedText,
    string Note);
