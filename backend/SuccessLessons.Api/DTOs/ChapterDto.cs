using SuccessLessons.Api.Models;

namespace SuccessLessons.Api.DTOs;

public record ChapterSummaryDto(
    string Id,
    string Title,
    string Slug,
    int Order,
    int EstimatedReadMinutes);

public record ChapterDetailDto(
    string Id,
    string PersonId,
    string Title,
    string Slug,
    int Order,
    List<ContentBlock> ContentBlocks,
    int EstimatedReadMinutes);
