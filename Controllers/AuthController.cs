using GnosisKernel.Api.Contracts;
using GnosisKernel.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GnosisKernel.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req, CancellationToken ct)
    {
        try
        {
            await auth.RegisterAsync(req, ct);
            return Created("", null);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req, CancellationToken ct)
    {
        var token = await auth.LoginAsync(req, ct);
        return token is null ? Unauthorized() : Ok(new AuthResponse(token));
    }
}