using GnosisKernel.Api.Data;
using GnosisKernel.Api.Storage;

namespace GnosisKernel.Api.Services;

public interface IUploadService
{
    Task<(Guid uploadId, Guid jobId)> UploadZipAndQueueJobAsync(Project project, IFormFile file, CancellationToken ct);
}

public sealed class UploadService(AppDbContext db, IFileStorage storage) : IUploadService
{
    public async Task<(Guid uploadId, Guid jobId)> UploadZipAndQueueJobAsync(Project project, IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0) throw new ArgumentException("Empty file.");
        if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Only .zip uploads supported.");

        await using var s = file.OpenReadStream();
        var storedPath = await storage.SaveAsync(s, ".zip", ct);

        var upload = new RepoUpload
        {
            ProjectId = project.Id,
            OriginalFileName = file.FileName,
            StoredPath = storedPath,
            SizeBytes = file.Length
        };
        db.RepoUploads.Add(upload);

        var job = new IndexJob
        {
            ProjectId = project.Id,
            RepoUploadId = upload.Id,
            Status = JobStatus.Queued,
            ProgressPct = 0,
            Message = "Queued for indexing."
        };
        db.IndexJobs.Add(job);

        project.Status = ProjectStatus.Indexing;

        await db.SaveChangesAsync(ct);

        return (upload.Id, job.Id);
    }
}