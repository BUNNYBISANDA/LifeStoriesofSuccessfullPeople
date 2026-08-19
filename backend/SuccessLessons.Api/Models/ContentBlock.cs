using Google.Cloud.Firestore;

namespace SuccessLessons.Api.Models;

[FirestoreData]
public class ContentBlock
{
    [FirestoreProperty("type")]
    public string Type { get; set; } = "paragraph"; // paragraph | quote | image | stat

    [FirestoreProperty("content")]
    public string Content { get; set; } = string.Empty;
}
