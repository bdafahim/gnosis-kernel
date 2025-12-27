namespace GnosisKernel.Api.Contracts;

public sealed record CreateProjectRequest(string Name, string? Description);
public sealed record ProjectResponse(Guid Id, string Name, string? Description, string Status, DateTimeOffset CreatedAt);