namespace GnosisKernel.Api.Contracts;

public sealed record JobProgressRequest(int ProgressPct, string? Message);
public sealed record JobFailRequest(string Error, string? Message);