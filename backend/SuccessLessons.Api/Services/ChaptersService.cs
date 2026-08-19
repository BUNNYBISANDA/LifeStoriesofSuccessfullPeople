using Google.Cloud.Firestore;
using SuccessLessons.Api.Models;

namespace SuccessLessons.Api.Services;

public class ChaptersService
{
    private readonly CollectionReference _chapters;

    public ChaptersService(IFirestoreService firestore)
    {
        _chapters = firestore.Db.Collection("chapters");
    }

    public async Task<Chapter?> GetBySlugAsync(string slug)
    {
        var snapshot = await _chapters.WhereEqualTo("slug", slug).Limit(1).GetSnapshotAsync();
        return snapshot.Documents.Count == 0 ? null : snapshot.Documents[0].ConvertTo<Chapter>();
    }

    public async Task<Chapter?> GetByIdAsync(string id)
    {
        var snapshot = await _chapters.Document(id).GetSnapshotAsync();
        return snapshot.Exists ? snapshot.ConvertTo<Chapter>() : null;
    }
}
