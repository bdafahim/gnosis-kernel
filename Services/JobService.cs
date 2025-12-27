using GnosisKernel.Api.Contracts;
using GnosisKernel.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GnosisKernel.Api.Services;

public interface IJobService
{
    Task<JobResponse?> GetLatestAsync(Guid projectId, CancellationToken ct);

    // internal updater methods (called by Python indexer)
    Task<bool> StartAsync(Guid jobId, CancellationToken ct);
    Task<bool> ProgressAsync(Guid jobId, JobProgressRequest req, CancellationToken ct);
    Task<bool> CompleteAsync(Guid jobId, string? message, CancellationToken ct);
    Task<bool> FailAsync(Guid jobId, JobFailRequest req, CancellationToken ct);
}

public sealed class JobService(AppDbContext db) : IJobService
{
    public async Task<JobResponse?> GetLatestAsync(Guid projectId, CancellationToken ct)
    {
        var job = await db.IndexJobs
            .Where(j => j.ProjectId == projectId)
            .OrderByDescending(j => j.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return job is null
            ? null
            : new JobResponse(job.Id, job.Status.ToString(), job.ProgressPct, job.Message, job.Error, job.CreatedAt);
    }

    public async Task<bool> StartAsync(Guid jobId, CancellationToken ct)
    {
        var job = await db.IndexJobs.SingleOrDefaultAsync(j => j.Id == jobId, ct);
        if (job is null) return false;

        if (job.Status is JobStatus.Succeeded or JobStatus.Failed) return true;

        job.Status = JobStatus.Running;
        job.StartedAt = DateTimeOffset.UtcNow;
        job.Message = "Indexing started.";
        job.Error = null;
        job.ProgressPct = Math.Max(job.ProgressPct, 1);

        // Keep project status consistent
        var project = await db.Projects.SingleOrDefaultAsync(p => p.Id == job.ProjectId, ct);
        if (project is not null) project.Status = ProjectStatus.Indexing;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ProgressAsync(Guid jobId, JobProgressRequest req, CancellationToken ct)
    {
        var job = await db.IndexJobs.SingleOrDefaultAsync(j => j.Id == jobId, ct);
        if (job is null) return false;

        if (job.Status is JobStatus.Succeeded or JobStatus.Failed) return true;

        job.Status = JobStatus.Running;
        job.ProgressPct = Math.Clamp(req.ProgressPct, 0, 100);
        job.Message = req.Message ?? job.Message;

        if (job.StartedAt is null) job.StartedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> CompleteAsync(Guid jobId, string? message, CancellationToken ct)
    {
        var job = await db.IndexJobs.SingleOrDefaultAsync(j => j.Id == jobId, ct);
        if (job is null) return false;

        job.Status = JobStatus.Succeeded;
        job.ProgressPct = 100;
        job.Message = message ?? "Indexing complete.";
        job.Error = null;
        job.FinishedAt = DateTimeOffset.UtcNow;
        if (job.StartedAt is null) job.StartedAt = job.FinishedAt;

        var project = await db.Projects.SingleOrDefaultAsync(p => p.Id == job.ProjectId, ct);
        if (project is not null) project.Status = ProjectStatus.Ready;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> FailAsync(Guid jobId, JobFailRequest req, CancellationToken ct)
    {
        var job = await db.IndexJobs.SingleOrDefaultAsync(j => j.Id == jobId, ct);
        if (job is null) return false;

        job.Status = JobStatus.Failed;
        job.Error = req.Error;
        job.Message = req.Message ?? "Indexing failed.";
        job.FinishedAt = DateTimeOffset.UtcNow;
        if (job.StartedAt is null) job.StartedAt = job.FinishedAt;

        var project = await db.Projects.SingleOrDefaultAsync(p => p.Id == job.ProjectId, ct);
        if (project is not null) project.Status = ProjectStatus.Failed;

        await db.SaveChangesAsync(ct);
        return true;
    }
}