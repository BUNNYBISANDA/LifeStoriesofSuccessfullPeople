using Google.Cloud.Firestore;

namespace SuccessLessons.Api.Models;

[FirestoreData]
public class Chapter
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("personId")]
    public string PersonId { get; set; } = string.Empty;

    [FirestoreProperty("title")]
    public string Title { get; set; } = string.Empty;

    [FirestoreProperty("slug")]
    public string Slug { get; set; } = string.Empty;

    [FirestoreProperty("order")]
    public int Order { get; set; }

    [FirestoreProperty("contentBlocks")]
    public List<ContentBlock> ContentBlocks { get; set; } = new();

    [FirestoreProperty("estimatedReadMinutes")]
    public int EstimatedReadMinutes { get; set; }

    [FirestoreProperty("createdAt")]
    public Timestamp CreatedAt { get; set; }
}
