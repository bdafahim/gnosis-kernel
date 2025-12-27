using GnosisKernel.Api.Auth;
using GnosisKernel.Api.Contracts;
using GnosisKernel.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GnosisKernel.Api.Controllers;

[ApiController]
[InternalKeyAuth]
[Route("internal/jobs")]
public sealed class InternalJobsController(IJobService jobs) : ControllerBase
{
    [HttpPost("{jobId:guid}/start")]
    public async Task<IActionResult> Start(Guid jobId, CancellationToken ct)
    {
        var ok = await jobs.StartAsync(jobId, ct);
        return ok ? Ok() : NotFound();
    }

    [HttpPost("{jobId:guid}/progress")]
    public async Task<IActionResult> Progress(Guid jobId, JobProgressRequest req, CancellationToken ct)
    {
        var ok = await jobs.ProgressAsync(jobId, req, ct);
        return ok ? Ok() : NotFound();
    }

    [HttpPost("{jobId:guid}/complete")]
    public async Task<IActionResult> Complete(Guid jobId, [FromBody] string? message, CancellationToken ct)
    {
        var ok = await jobs.CompleteAsync(jobId, message, ct);
        return ok ? Ok() : NotFound();
    }

    [HttpPost("{jobId:guid}/fail")]
    public async Task<IActionResult> Fail(Guid jobId, JobFailRequest req, CancellationToken ct)
    {
        var ok = await jobs.FailAsync(jobId, req, ct);
        return ok ? Ok() : NotFound();
    }
}