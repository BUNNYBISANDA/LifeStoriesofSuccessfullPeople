using Google.Cloud.Firestore;

namespace SuccessLessons.Api.Models;

[FirestoreData]
public class Quote
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("personId")]
    public string PersonId { get; set; } = string.Empty;

    [FirestoreProperty("text")]
    public string Text { get; set; } = string.Empty;

    [FirestoreProperty("context")]
    public string Context { get; set; } = string.Empty;
}
