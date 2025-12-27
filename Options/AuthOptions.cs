namespace GnosisKernel.Api.Options;

public sealed class AuthOptions
{
    public required string JwtIssuer { get; init; }
    public required string JwtAudience { get; init; }
    public required string JwtKey { get; init; }
}