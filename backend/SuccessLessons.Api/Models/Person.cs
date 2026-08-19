using Google.Cloud.Firestore;

namespace SuccessLessons.Api.Models;

[FirestoreData]
public class Person
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("name")]
    public string Name { get; set; } = string.Empty;

    [FirestoreProperty("slug")]
    public string Slug { get; set; } = string.Empty;

    [FirestoreProperty("era")]
    public string Era { get; set; } = string.Empty;

    [FirestoreProperty("field")]
    public string Field { get; set; } = string.Empty;

    [FirestoreProperty("summary")]
    public string Summary { get; set; } = string.Empty;

    [FirestoreProperty("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [FirestoreProperty("failureCount")]
    public int FailureCount { get; set; }

    [FirestoreProperty("tags")]
    public List<string> Tags { get; set; } = new();

    [FirestoreProperty("createdAt")]
    public Timestamp CreatedAt { get; set; }
}
