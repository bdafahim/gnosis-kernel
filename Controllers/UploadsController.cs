using System.Security.Claims;
using GnosisKernel.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GnosisKernel.Api.Controllers;

[ApiController]
[Authorize]
[Route("projects/{projectId:guid}/uploads")]
public sealed class UploadsController(IProjectService projects, IUploadService uploads) : ControllerBase
{
    [HttpPost]
    [DisableRequestSizeLimit] // or set a reasonable max; we can add validation later
    public async Task<IActionResult> Upload(Guid projectId, IFormFile file, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var project = await projects.GetOwnedAsync(userId, projectId, ct);
        if (project is null) return NotFound("Project not found.");

        try
        {
            var (uploadId, jobId) = await uploads.UploadZipAndQueueJobAsync(project, file, ct);
            return Accepted(new { uploadId, jobId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}