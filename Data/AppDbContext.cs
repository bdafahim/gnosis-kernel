using Microsoft.EntityFrameworkCore;

namespace GnosisKernel.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<RepoUpload> RepoUploads => Set<RepoUpload>();
    public DbSet<IndexJob> IndexJobs => Set<IndexJob>();
    public DbSet<CodeFile> CodeFiles => Set<CodeFile>();
    public DbSet<CodeSymbol> CodeSymbols => Set<CodeSymbol>();
    public DbSet<CodeEdge> CodeEdges => Set<CodeEdge>();
    public DbSet<CodeChunk> CodeChunks => Set<CodeChunk>();
    public DbSet<ChunkEmbedding> ChunkEmbeddings => Set<ChunkEmbedding>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasPostgresExtension("vector");

        b.Entity<User>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(320);
        });

        b.Entity<Project>(e =>
        {
            e.HasIndex(x => new { x.OwnerUserId, x.Name }).IsUnique();
        });

        b.Entity<CodeFile>(e =>
        {
            e.HasIndex(x => new { x.ProjectId, x.Path }).IsUnique();
        });

        b.Entity<CodeChunk>(e =>
        {
            e.HasIndex(x => new { x.ProjectId, x.FileId, x.StartLine, x.EndLine });
        });

        b.Entity<ChunkEmbedding>(e =>
        {
            e.Property(x => x.Embedding).HasColumnType("vector(1536)");
            e.HasIndex(x => x.ProjectId);
            e.HasIndex(x => x.ChunkId).IsUnique();
        });
    }
}