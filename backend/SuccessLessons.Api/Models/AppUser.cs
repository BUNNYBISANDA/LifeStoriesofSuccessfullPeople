using Google.Cloud.Firestore;

namespace SuccessLessons.Api.Models;

[FirestoreData]
public class AppUser
{
    [FirestoreDocumentId]
    public string Uid { get; set; } = string.Empty;

    [FirestoreProperty("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [FirestoreProperty("email")]
    public string Email { get; set; } = string.Empty;

    [FirestoreProperty("joinedAt")]
    public Timestamp JoinedAt { get; set; }

    [FirestoreProperty("readingStreak")]
    public int ReadingStreak { get; set; }
}
