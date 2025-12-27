using System.Security.Claims;
using GnosisKernel.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GnosisKernel.Api.Controllers;

[ApiController]
[Authorize]
[Route("projects/{projectId:guid}")]
public sealed class ArtifactsController(
    IProjectService projects,
    IArtifactService artifacts) : ControllerBase
{
    [HttpGet("files")]
    public async Task<IActionResult> Files(Guid projectId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var project = await projects.GetOwnedAsync(userId, projectId, ct);
        if (project is null) return NotFound();

        var files = await artifacts.ListFilesAsync(projectId, ct);
        return Ok(files);
    }

    [HttpGet("graph")]
    public async Task<IActionResult> Graph(Guid projectId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var project = await projects.GetOwnedAsync(userId, projectId, ct);
        if (project is null) return NotFound();

        var graph = await artifacts.GetGraphAsync(projectId, ct);
        return Ok(graph);
    }

    // Optional but useful for FE later (limit/offset to avoid huge payloads)
    // GET /projects/{id}/chunks?limit=200&offset=0&fileId=<guid>
    [HttpGet("chunks")]
    public async Task<IActionResult> Chunks(
        Guid projectId,
        [FromQuery] int limit = 200,
        [FromQuery] int offset = 0,
        [FromQuery] Guid? fileId = null,
        CancellationToken ct = default)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var project = await projects.GetOwnedAsync(userId, projectId, ct);
        if (project is null) return NotFound();

        var chunks = await artifacts.ListChunksAsync(projectId, limit, offset, fileId, ct);
        return Ok(chunks);
    }
}