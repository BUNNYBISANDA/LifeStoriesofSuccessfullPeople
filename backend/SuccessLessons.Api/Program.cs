using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using SuccessLessons.Api.Config;
using SuccessLessons.Api.Middleware;
using SuccessLessons.Api.Services;

// Cloud hosts (Render, etc.) can't easily mount a credentials file — let the
// service account key be supplied as a raw JSON env var instead, written to a
// temp file so the existing CredentialsPath-based code paths keep working.
var firebaseCredentialsJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
if (!string.IsNullOrEmpty(firebaseCredentialsJson))
{
    var tempCredentialsPath = Path.Combine(Path.GetTempPath(), "firebase-service-account.json");
    File.WriteAllText(tempCredentialsPath, firebaseCredentialsJson);
    Environment.SetEnvironmentVariable("Firebase__CredentialsPath", tempCredentialsPath);
}

var builder = WebApplication.CreateBuilder(args);

// Sentry.AspNetCore requires an explicit empty string to disable itself —
// an absent/null Dsn throws at startup instead of no-op'ing (e.g. local dev
// where SENTRY_DSN isn't set).
builder.WebHost.UseSentry(options =>
{
    options.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN") ?? "";
    options.TracesSampleRate = 0.2;
});

// Render (and most container hosts) assign the listen port via $PORT at runtime.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// --- Firebase Admin SDK ---
var firebaseConfig = builder.Configuration.GetSection(FirebaseOptions.SectionName).Get<FirebaseOptions>()
    ?? new FirebaseOptions();

builder.Services.Configure<FirebaseOptions>(builder.Configuration.GetSection(FirebaseOptions.SectionName));

if (FirebaseApp.DefaultInstance is null)
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = !string.IsNullOrEmpty(firebaseConfig.CredentialsPath)
            ? GoogleCredential.FromFile(firebaseConfig.CredentialsPath)
            : GoogleCredential.GetApplicationDefault(),
        ProjectId = firebaseConfig.ProjectId
    });
}

// --- Dependency Injection ---
builder.Services.AddSingleton<IFirestoreService, FirestoreService>();
builder.Services.AddScoped<PeopleService>();
builder.Services.AddScoped<ChaptersService>();
builder.Services.AddScoped<LessonsService>();
builder.Services.AddScoped<BookmarkService>();
builder.Services.AddScoped<ProgressService>();
builder.Services.AddScoped<HighlightService>();
builder.Services.AddScoped<UserService>();

// --- Auth ---
builder.Services
    .AddAuthentication(FirebaseAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(FirebaseAuthenticationHandler.SchemeName, null);
builder.Services.AddAuthorization();

// --- CORS ---
const string CorsPolicy = "Frontend";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.SetIsOriginAllowed(origin =>
                  allowedOrigins.Contains(origin) ||
                  Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase))
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();

app.UseCors(CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
