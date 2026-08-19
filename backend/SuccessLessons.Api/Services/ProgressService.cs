using Google.Cloud.Firestore;
using SuccessLessons.Api.Models;

namespace SuccessLessons.Api.Services;

public class ProgressService
{
    private readonly CollectionReference _progress;

    public ProgressService(IFirestoreService firestore)
    {
        _progress = firestore.Db.Collection("readingProgress");
    }

    private static string DocId(string uid, string chapterId) => $"{uid}_{chapterId}";

    public async Task<List<ReadingProgress>> GetForUserAsync(string uid)
    {
        var snapshot = await _progress.WhereEqualTo("uid", uid).GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<ReadingProgress>()).ToList();
    }

    public async Task<ReadingProgress?> GetForChapterAsync(string uid, string chapterId)
    {
        var snapshot = await _progress.Document(DocId(uid, chapterId)).GetSnapshotAsync();
        return snapshot.Exists ? snapshot.ConvertTo<ReadingProgress>() : null;
    }

    public async Task<ReadingProgress> UpsertAsync(string uid, string chapterId, double percentComplete, int lastPositionBlockIndex)
    {
        var progress = new ReadingProgress
        {
            Id = DocId(uid, chapterId),
            Uid = uid,
            ChapterId = chapterId,
            PercentComplete = percentComplete,
            LastPositionBlockIndex = lastPositionBlockIndex,
            Completed = percentComplete >= 100,
            UpdatedAt = Timestamp.GetCurrentTimestamp()
        };

        await _progress.Document(progress.Id).SetAsync(progress);
        return progress;
    }
}
