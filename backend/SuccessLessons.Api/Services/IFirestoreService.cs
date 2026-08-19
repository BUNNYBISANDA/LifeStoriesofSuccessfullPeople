using Google.Cloud.Firestore;

namespace SuccessLessons.Api.Services;

public interface IFirestoreService
{
    FirestoreDb Db { get; }
}
