using Google.Cloud.Firestore;
using SuccessLessons.Api.Models;

namespace SuccessLessons.Api.Services;

public class PeopleService
{
    private readonly CollectionReference _people;
    private readonly CollectionReference _chapters;

    public PeopleService(IFirestoreService firestore)
    {
        _people = firestore.Db.Collection("people");
        _chapters = firestore.Db.Collection("chapters");
    }

    public async Task<List<Person>> GetAllAsync(string? field = null, string? tag = null)
    {
        Query query = _people;
        if (!string.IsNullOrEmpty(field))
            query = query.WhereEqualTo("field", field);
        if (!string.IsNullOrEmpty(tag))
            query = query.WhereArrayContains("tags", tag);

        var snapshot = await query.GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Person>()).ToList();
    }

    public async Task<Person?> GetBySlugAsync(string slug)
    {
        var snapshot = await _people.WhereEqualTo("slug", slug).Limit(1).GetSnapshotAsync();
        return snapshot.Documents.Count == 0 ? null : snapshot.Documents[0].ConvertTo<Person>();
    }

    public async Task<List<Chapter>> GetChaptersForPersonAsync(string personId)
    {
        var snapshot = await _chapters
            .WhereEqualTo("personId", personId)
            .OrderBy("order")
            .GetSnapshotAsync();

        return snapshot.Documents.Select(d => d.ConvertTo<Chapter>()).ToList();
    }
}
