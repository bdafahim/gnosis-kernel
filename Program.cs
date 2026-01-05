using System.Text;
using GnosisKernel.Api.Auth;
using GnosisKernel.Api.Data;
using GnosisKernel.Api.Options;
using GnosisKernel.Api.Services;
using GnosisKernel.Api.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pgvector.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Options
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));

// Db
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        npgsql =>
        {
            npgsql.EnableRetryOnFailure();
            npgsql.UseVector();
        });
});

// Auth DI
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// Services DI
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.Configure<InternalOptions>(builder.Configuration.GetSection("Internal"));
builder.Services.AddScoped<IArtifactService, ArtifactService>();

// Storage DI
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("dev", p =>
        p.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// JWT setup
var auth = builder.Configuration.GetSection("Auth").Get<AuthOptions>()!;
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(auth.JwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = auth.JwtIssuer,
            ValidAudience = auth.JwtAudience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IChatService, ChatService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { ok = true }));

app.UseCors("dev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();