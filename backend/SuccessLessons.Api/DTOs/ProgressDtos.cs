namespace SuccessLessons.Api.DTOs;

public record ProgressDto(
    string ChapterId,
    double PercentComplete,
    int LastPositionBlockIndex,
    bool Completed,
    DateTime UpdatedAt);

public record UpsertProgressRequest(
    double PercentComplete,
    int LastPositionBlockIndex);
