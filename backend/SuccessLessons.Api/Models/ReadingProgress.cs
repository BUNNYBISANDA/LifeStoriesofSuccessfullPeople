using Google.Cloud.Firestore;

namespace SuccessLessons.Api.Models;

[FirestoreData]
public class ReadingProgress
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty; // {uid}_{chapterId}

    [FirestoreProperty("uid")]
    public string Uid { get; set; } = string.Empty;

    [FirestoreProperty("chapterId")]
    public string ChapterId { get; set; } = string.Empty;

    [FirestoreProperty("percentComplete")]
    public double PercentComplete { get; set; }

    [FirestoreProperty("lastPositionBlockIndex")]
    public int LastPositionBlockIndex { get; set; }

    [FirestoreProperty("completed")]
    public bool Completed { get; set; }

    [FirestoreProperty("updatedAt")]
    public Timestamp UpdatedAt { get; set; }
}
