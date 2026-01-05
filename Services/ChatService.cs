using GnosisKernel.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GnosisKernel.Api.Services;

public interface IChatService
{
    Task<ChatResponse> ChatAsync(Guid projectId, string message, CancellationToken ct);
}

public sealed class ChatService(AppDbContext db) : IChatService
{
    public async Task<ChatResponse> ChatAsync(Guid projectId, string message, CancellationToken ct)
    {
        var files = await db.CodeFiles
            .Where(f => f.ProjectId == projectId)
            .Select(f => f.Path)
            .Take(3)
            .ToListAsync(ct);

        // Fake “reasoned” response
        var answer =
            $"""
             Based on your project structure, here is a summary:

             • The project contains {files.Count} main files
             • The question was: "{message}"

             This is a simulated response. In a real agent, this would be grounded in code analysis and embeddings.
             """;

        return new ChatResponse(
            Answer: answer,
            Sources: files
        );
    }
}