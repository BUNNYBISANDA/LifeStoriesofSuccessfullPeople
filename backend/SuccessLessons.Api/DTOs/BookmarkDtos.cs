namespace SuccessLessons.Api.DTOs;

public record BookmarkDto(string Id, string ChapterId, DateTime CreatedAt);

public record CreateBookmarkRequest(string ChapterId);
