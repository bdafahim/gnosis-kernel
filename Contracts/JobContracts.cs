namespace GnosisKernel.Api.Contracts;

public sealed record JobResponse(Guid Id, string Status, int ProgressPct, string? Message, string? Error, DateTimeOffset CreatedAt);