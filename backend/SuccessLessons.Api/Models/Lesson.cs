using Google.Cloud.Firestore;

namespace SuccessLessons.Api.Models;

[FirestoreData]
public class Lesson
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("personId")]
    public string PersonId { get; set; } = string.Empty;

    [FirestoreProperty("chapterId")]
    public string ChapterId { get; set; } = string.Empty;

    [FirestoreProperty("text")]
    public string Text { get; set; } = string.Empty;

    [FirestoreProperty("category")]
    public string Category { get; set; } = string.Empty; // failure | passion | hard-work | mindset

    [FirestoreProperty("isFeatured")]
    public bool IsFeatured { get; set; }
}
