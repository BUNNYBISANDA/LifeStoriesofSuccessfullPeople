using Google.Cloud.Firestore;
using Microsoft.Extensions.Options;
using SuccessLessons.Api.Config;

namespace SuccessLessons.Api.Services;

public class FirestoreService : IFirestoreService
{
    public FirestoreDb Db { get; }

    public FirestoreService(IOptions<FirebaseOptions> options)
    {
        var config = options.Value;

        if (!string.IsNullOrEmpty(config.CredentialsPath))
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", config.CredentialsPath);
        }

        Db = new FirestoreDbBuilder
        {
            ProjectId = config.ProjectId
        }.Build();
    }
}
