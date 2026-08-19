using Google.Cloud.Firestore;
using SuccessLessons.Api.Models;

namespace SuccessLessons.Api.Services;

public class HighlightService
{
    private readonly CollectionReference _highlights;

    public HighlightService(IFirestoreService firestore)
    {
        _highlights = firestore.Db.Collection("highlights");
    }

    public async Task<List<Highlight>> GetForChapterAsync(string uid, string chapterId)
    {
        var snapshot = await _highlights
            .WhereEqualTo("uid", uid)
            .WhereEqualTo("chapterId", chapterId)
            .GetSnapshotAsync();

        return snapshot.Documents.Select(d => d.ConvertTo<Highlight>()).ToList();
    }

    public async Task<Highlight> AddAsync(string uid, string chapterId, int blockIndex, string selectedText, string note)
    {
        var highlight = new Highlight
        {
            Uid = uid,
            ChapterId = chapterId,
            BlockIndex = blockIndex,
            SelectedText = selectedText,
            Note = note,
            CreatedAt = Timestamp.GetCurrentTimestamp()
        };

        var docRef = await _highlights.AddAsync(highlight);
        highlight.Id = docRef.Id;
        return highlight;
    }

    public async Task<bool> DeleteAsync(string uid, string highlightId)
    {
        var docRef = _highlights.Document(highlightId);
        var snapshot = await docRef.GetSnapshotAsync();
        if (!snapshot.Exists) return false;

        var highlight = snapshot.ConvertTo<Highlight>();
        if (highlight.Uid != uid) return false;

        await docRef.DeleteAsync();
        return true;
    }
}
