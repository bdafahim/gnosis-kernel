using GnosisKernel.Api.Contracts;
using GnosisKernel.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GnosisKernel.Api.Services;

public interface IArtifactService
{
    Task<IReadOnlyList<FileResponse>> ListFilesAsync(Guid projectId, CancellationToken ct);
    Task<GraphResponse> GetGraphAsync(Guid projectId, CancellationToken ct);
    Task<IReadOnlyList<ChunkResponse>> ListChunksAsync(Guid projectId, int limit, int offset, Guid? fileId, CancellationToken ct);
}

public sealed class ArtifactService(AppDbContext db) : IArtifactService
{
    public async Task<IReadOnlyList<FileResponse>> ListFilesAsync(Guid projectId, CancellationToken ct)
    {
        var files = await db.CodeFiles
            .Where(f => f.ProjectId == projectId)
            .OrderBy(f => f.Path)
            .Select(f => new FileResponse(f.Id, f.Path, f.Language, f.SizeBytes))
            .ToListAsync(ct);

        return files;
    }

    public async Task<GraphResponse> GetGraphAsync(Guid projectId, CancellationToken ct)
    {
        // Minimal node metadata: Symbol + file path
        var nodes = await (
            from s in db.CodeSymbols
            join f in db.CodeFiles on s.FileId equals f.Id
            where s.ProjectId == projectId
            select new GraphNodeDto(
                s.Id,
                s.Name,
                s.Kind.ToString(),
                f.Id,
                f.Path
            )
        ).ToListAsync(ct);

        var edges = await db.CodeEdges
            .Where(e => e.ProjectId == projectId)
            .Select(e => new GraphEdgeDto(e.FromSymbolId, e.ToSymbolId, e.Type.ToString()))
            .ToListAsync(ct);

        return new GraphResponse(nodes, edges);
    }

    public async Task<IReadOnlyList<ChunkResponse>> ListChunksAsync(Guid projectId, int limit, int offset, Guid? fileId, CancellationToken ct)
    {
        limit = Math.Clamp(limit, 1, 500);      // protect API
        offset = Math.Max(offset, 0);

        var q =
            from c in db.CodeChunks
            join f in db.CodeFiles on c.FileId equals f.Id
            where c.ProjectId == projectId
            select new { c, f };

        if (fileId is not null)
            q = q.Where(x => x.f.Id == fileId);

        var res = await q
            .OrderBy(x => x.f.Path).ThenBy(x => x.c.StartLine)
            .Skip(offset)
            .Take(limit)
            .Select(x => new ChunkResponse(
                x.c.Id,
                x.f.Id,
                x.f.Path,
                x.c.StartLine,
                x.c.EndLine,
                x.c.Content
            ))
            .ToListAsync(ct);

        return res;
    }
}