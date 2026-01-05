using GnosisKernel.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GnosisKernel.Api.Controllers;

[ApiController]
[Route("projects/{projectId:guid}/chat")]
[Authorize]
public sealed class ChatController(IChatService chat) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Chat(
        Guid projectId,
        [FromBody] ChatRequest req,
        CancellationToken ct)
    {
        var res = await chat.ChatAsync(projectId, req.Message, ct);
        return Ok(res);
    }
}