using GnosisKernel.Api.Contracts;
using GnosisKernel.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GnosisKernel.Api.Services;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(Guid projectId, string query, int k, CancellationToken ct);
}

public sealed class SearchService(AppDbContext db) : ISearchService
{
    public async Task<SearchResponse> SearchAsync(Guid projectId, string query, int k, CancellationToken ct)
    {
        k = Math.Clamp(k, 1, 20);
        query = query.Trim().ToLowerInvariant();

        // Simple keyword-based fake relevance
        var chunks = await (
            from c in db.CodeChunks
            join f in db.CodeFiles on c.FileId equals f.Id
            where c.ProjectId == projectId
            select new { c, f }
        ).ToListAsync(ct);

        var hits = chunks
            .Select(x =>
            {
                var content = x.c.Content.ToLowerInvariant();
                var score =
                    content.Contains(query) ? 0.95 :
                    x.f.Path.ToLowerInvariant().Contains(query) ? 0.75 :
                    0.35;

                return new SearchHitDto(
                    ChunkId: x.c.Id,
                    FileId: x.f.Id,
                    Path: x.f.Path,
                    StartLine: x.c.StartLine,
                    EndLine: x.c.EndLine,
                    Snippet: Snippet(x.c.Content),
                    Score: score
                );
            })
            .OrderByDescending(h => h.Score)
            .Take(k)
            .ToList();

        return new SearchResponse(query, hits);
    }

    private static string Snippet(string s)
    {
        s = s.Trim();
        return s.Length <= 500 ? s : s[..500] + "…";
    }
}