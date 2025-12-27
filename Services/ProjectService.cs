using GnosisKernel.Api.Contracts;
using GnosisKernel.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GnosisKernel.Api.Services;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(Guid userId, CreateProjectRequest req, CancellationToken ct);
    Task<IReadOnlyList<ProjectResponse>> ListAsync(Guid userId, CancellationToken ct);
    Task<Project?> GetOwnedAsync(Guid userId, Guid projectId, CancellationToken ct);
}

public sealed class ProjectService(AppDbContext db) : IProjectService
{
    public async Task<ProjectResponse> CreateAsync(Guid userId, CreateProjectRequest req, CancellationToken ct)
    {
        var project = new Project
        {
            OwnerUserId = userId,
            Name = req.Name.Trim(),
            Description = req.Description
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);

        return ToResponse(project);
    }

    public async Task<IReadOnlyList<ProjectResponse>> ListAsync(Guid userId, CancellationToken ct)
    {
        var projects = await db.Projects
            .Where(p => p.OwnerUserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

        return projects.Select(ToResponse).ToList();
    }

    public Task<Project?> GetOwnedAsync(Guid userId, Guid projectId, CancellationToken ct) =>
        db.Projects.SingleOrDefaultAsync(p => p.Id == projectId && p.OwnerUserId == userId, ct);

    private static ProjectResponse ToResponse(Project p) =>
        new(p.Id, p.Name, p.Description, p.Status.ToString(), p.CreatedAt);
}