using Pgvector;

namespace GnosisKernel.Api.Data;

public enum ProjectStatus { Created = 0, Indexing = 1, Ready = 2, Failed = 3 }
public enum JobStatus { Queued = 0, Running = 1, Succeeded = 2, Failed = 3 }
public enum SymbolKind { Unknown = 0, File = 1, Function = 2, Class = 3, Interface = 4, Type = 5 }
public enum EdgeType { Unknown = 0, Imports = 1, Calls = 2, Implements = 3, Reads = 4, Writes = 5 }

public sealed class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerUserId { get; set; }
    public required string Name { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Created;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class RepoUpload
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public required string OriginalFileName { get; set; }
    public required string StoredPath { get; set; }
    public long SizeBytes { get; set; }
    public string? Sha256 { get; set; }
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class IndexJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Guid? RepoUploadId { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Queued;
    public int ProgressPct { get; set; } = 0;
    public string? Message { get; set; }
    public string? Error { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
}

public sealed class CodeFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public required string Path { get; set; }
    public string? Language { get; set; }
    public string? Sha256 { get; set; }
    public long SizeBytes { get; set; }
}

public sealed class CodeSymbol
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Guid FileId { get; set; }
    public required string Name { get; set; }
    public SymbolKind Kind { get; set; } = SymbolKind.Unknown;
    public int StartLine { get; set; }
    public int EndLine { get; set; }
    public string? Signature { get; set; }
}

public sealed class CodeEdge
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Guid FromSymbolId { get; set; }
    public Guid ToSymbolId { get; set; }
    public EdgeType Type { get; set; } = EdgeType.Unknown;
}

public sealed class CodeChunk
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Guid FileId { get; set; }
    public int StartLine { get; set; }
    public int EndLine { get; set; }
    public required string Content { get; set; }
    public int TokenCount { get; set; }
}

public sealed class ChunkEmbedding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Guid ChunkId { get; set; }
    public Vector? Embedding { get; set; }
}