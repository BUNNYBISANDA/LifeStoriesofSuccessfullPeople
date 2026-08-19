using Google.Cloud.Firestore;
using SuccessLessons.Api.Models;

namespace SuccessLessons.Api.Services;

public class LessonsService
{
    private readonly CollectionReference _lessons;
    private readonly CollectionReference _quotes;

    public LessonsService(IFirestoreService firestore)
    {
        _lessons = firestore.Db.Collection("lessons");
        _quotes = firestore.Db.Collection("quotes");
    }

    public async Task<List<Lesson>> GetAllAsync(string? category = null)
    {
        Query query = _lessons;
        if (!string.IsNullOrEmpty(category))
            query = query.WhereEqualTo("category", category);

        var snapshot = await query.GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Lesson>()).ToList();
    }

    public async Task<List<Lesson>> GetFeaturedAsync()
    {
        var snapshot = await _lessons.WhereEqualTo("isFeatured", true).GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Lesson>()).ToList();
    }

    public async Task<Quote?> GetRandomQuoteAsync()
    {
        var snapshot = await _quotes.GetSnapshotAsync();
        if (snapshot.Documents.Count == 0) return null;
        var random = new Random();
        var doc = snapshot.Documents[random.Next(snapshot.Documents.Count)];
        return doc.ConvertTo<Quote>();
    }
}
