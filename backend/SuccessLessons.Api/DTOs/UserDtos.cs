namespace SuccessLessons.Api.DTOs;

public record UserProfileDto(
    string Uid,
    string DisplayName,
    string Email,
    DateTime JoinedAt,
    int ReadingStreak);

public record UpdateUserProfileRequest(string DisplayName);
