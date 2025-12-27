using GnosisKernel.Api.Options;
using Microsoft.Extensions.Options;

namespace GnosisKernel.Api.Storage;

public sealed class LocalFileStorage(IOptions<StorageOptions> opt) : IFileStorage
{
    private readonly StorageOptions _o = opt.Value;

    public async Task<string> SaveAsync(Stream content, string extension, CancellationToken ct)
    {
        Directory.CreateDirectory(_o.UploadsPath);

        var id = Guid.NewGuid();
        var fileName = $"{id}{extension}";
        var fullPath = Path.GetFullPath(Path.Combine(_o.UploadsPath, fileName));

        await using var fs = File.Create(fullPath);
        await content.CopyToAsync(fs, ct);

        return fullPath;
    }
}