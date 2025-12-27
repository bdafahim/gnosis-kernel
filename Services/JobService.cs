using GnosisKernel.Api.Contracts;
using GnosisKernel.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GnosisKernel.Api.Services;

public interface IJobService
{
    Task<JobResponse?> GetLatestAsync(Guid projectId, CancellationToken ct);
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
}