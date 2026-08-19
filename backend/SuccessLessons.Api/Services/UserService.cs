using Google.Cloud.Firestore;
using SuccessLessons.Api.Models;

namespace SuccessLessons.Api.Services;

public class UserService
{
    private readonly CollectionReference _users;

    public UserService(IFirestoreService firestore)
    {
        _users = firestore.Db.Collection("users");
    }

    public async Task<AppUser> GetOrCreateAsync(string uid, string email, string displayName)
    {
        var docRef = _users.Document(uid);
        var snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists) return snapshot.ConvertTo<AppUser>();

        var user = new AppUser
        {
            Uid = uid,
            Email = email,
            DisplayName = displayName,
            JoinedAt = Timestamp.GetCurrentTimestamp(),
            ReadingStreak = 0
        };

        await docRef.SetAsync(user);
        return user;
    }

    public async Task<AppUser> UpdateDisplayNameAsync(string uid, string displayName)
    {
        var docRef = _users.Document(uid);
        await docRef.UpdateAsync("displayName", displayName);
        var snapshot = await docRef.GetSnapshotAsync();
        return snapshot.ConvertTo<AppUser>();
    }
}
