using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using SuccessLessons.Api.Config;
using SuccessLessons.Api.Middleware;
using SuccessLessons.Api.Services;

var builder = WebApplication.CreateBuilder(args);

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
        policy.WithOrigins(allowedOrigins)
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

app.UseHttpsRedirection();

app.UseCors(CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
