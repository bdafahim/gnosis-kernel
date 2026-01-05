using GnosisKernel.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GnosisKernel.Api.Controllers;

[ApiController]
[Route("projects/{projectId:guid}/search")]
[Authorize]
public sealed class SearchController(ISearchService search) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search(
        Guid projectId,
        [FromQuery] string q,
        [FromQuery] int k = 10,
        CancellationToken ct = default)
    {
        var res = await search.SearchAsync(projectId, q, k, ct);
        return Ok(res);
    }
}