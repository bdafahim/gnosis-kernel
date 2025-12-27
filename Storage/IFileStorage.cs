namespace GnosisKernel.Api.Storage;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream content, string extension, CancellationToken ct);
}