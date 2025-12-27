using GnosisKernel.Api.Auth;
using GnosisKernel.Api.Contracts;
using GnosisKernel.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GnosisKernel.Api.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest req, CancellationToken ct);
    Task<string?> LoginAsync(LoginRequest req, CancellationToken ct);
}

public sealed class AuthService(
    AppDbContext db,
    IPasswordHasher hasher,
    IJwtTokenService jwt) : IAuthService
{
    public async Task RegisterAsync(RegisterRequest req, CancellationToken ct)
    {
        var email = NormalizeEmail(req.Email);

        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Email = email,
            PasswordHash = hasher.Hash(req.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
    }

    public async Task<string?> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var email = NormalizeEmail(req.Email);

        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email, ct);
        if (user is null) return null;

        if (!hasher.Verify(req.Password, user.PasswordHash)) return null;

        return jwt.CreateToken(user.Id, user.Email);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}