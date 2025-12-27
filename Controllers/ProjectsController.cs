using System.Security.Claims;
using GnosisKernel.Api.Contracts;
using GnosisKernel.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GnosisKernel.Api.Controllers;

[ApiController]
[Authorize]
[Route("projects")]
public sealed class ProjectsController(IProjectService projects) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(CreateProjectRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var created = await projects.CreateAsync(userId, req, ct);
        return CreatedAtAction(nameof(GetById), new { projectId = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>> List(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await projects.ListAsync(userId, ct);
        return Ok(list);
    }

    [HttpGet("{projectId:guid}")]
    public async Task<IActionResult> GetById(Guid projectId, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var project = await projects.GetOwnedAsync(userId, projectId, ct);
        return project is null ? NotFound() : Ok(project);
    }
}