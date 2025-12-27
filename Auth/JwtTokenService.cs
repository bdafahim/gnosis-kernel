using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GnosisKernel.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GnosisKernel.Api.Auth;

public interface IJwtTokenService
{
    string CreateToken(Guid userId, string email);
}

public sealed class JwtTokenService(IOptions<AuthOptions> opt) : IJwtTokenService
{
    private readonly AuthOptions _o = opt.Value;

    public string CreateToken(Guid userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_o.JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email)
        };

        var token = new JwtSecurityToken(
            issuer: _o.JwtIssuer,
            audience: _o.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}