using System.Security.Claims;
using GnosisKernel.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GnosisKernel.Api.Controllers;

[ApiController]
[Authorize]
[Route("projects/{projectId:guid}/jobs")]
public sealed class JobsController(IProjectService projects, IJobService jobs) : ControllerBase
{
    [HttpGet("latest")]
    public async Task<IActionResult> Latest(Guid projectId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var project = await projects.GetOwnedAsync(userId, projectId, ct);
        if (project is null) return NotFound();

        var job = await jobs.GetLatestAsync(projectId, ct);
        return job is null ? NotFound() : Ok(job);
    }
}