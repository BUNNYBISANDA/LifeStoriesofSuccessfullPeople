using Google.Cloud.Firestore;
using SuccessLessons.Api.Models;

namespace SuccessLessons.Api.Services;

public class BookmarkService
{
    private readonly CollectionReference _bookmarks;

    public BookmarkService(IFirestoreService firestore)
    {
        _bookmarks = firestore.Db.Collection("bookmarks");
    }

    public async Task<List<Bookmark>> GetForUserAsync(string uid)
    {
        var snapshot = await _bookmarks.WhereEqualTo("uid", uid).GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Bookmark>()).ToList();
    }

    public async Task<Bookmark> AddAsync(string uid, string chapterId)
    {
        var bookmark = new Bookmark
        {
            Uid = uid,
            ChapterId = chapterId,
            CreatedAt = Timestamp.GetCurrentTimestamp()
        };

        var docRef = await _bookmarks.AddAsync(bookmark);
        bookmark.Id = docRef.Id;
        return bookmark;
    }

    public async Task<bool> DeleteAsync(string uid, string bookmarkId)
    {
        var docRef = _bookmarks.Document(bookmarkId);
        var snapshot = await docRef.GetSnapshotAsync();
        if (!snapshot.Exists) return false;

        var bookmark = snapshot.ConvertTo<Bookmark>();
        if (bookmark.Uid != uid) return false;

        await docRef.DeleteAsync();
        return true;
    }
}
