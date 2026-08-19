using Google.Cloud.Firestore;

namespace SuccessLessons.Api.Models;

[FirestoreData]
public class Highlight
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("uid")]
    public string Uid { get; set; } = string.Empty;

    [FirestoreProperty("chapterId")]
    public string ChapterId { get; set; } = string.Empty;

    [FirestoreProperty("blockIndex")]
    public int BlockIndex { get; set; }

    [FirestoreProperty("selectedText")]
    public string SelectedText { get; set; } = string.Empty;

    [FirestoreProperty("note")]
    public string Note { get; set; } = string.Empty;

    [FirestoreProperty("createdAt")]
    public Timestamp CreatedAt { get; set; }
}
