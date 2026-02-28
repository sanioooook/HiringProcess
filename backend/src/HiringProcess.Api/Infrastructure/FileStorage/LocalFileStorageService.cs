namespace HiringProcess.Api.Infrastructure.FileStorage;

/// <summary>
/// Stores uploaded files on the local filesystem under /uploads.
/// In production, replace with an S3/Azure Blob implementation of IFileStorageService.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".txt" };

    public LocalFileStorageService(IConfiguration configuration)
    {
        _basePath = configuration["FileStorage:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveAsync(Stream fileStream, string originalFileName, CancellationToken ct = default)
    {
        var extension = Path.GetExtension(originalFileName);

        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException($"File type '{extension}' is not allowed. Only PDF and TXT files are accepted.");

        // Unique name to avoid collisions
        var storedName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_basePath, storedName);

        await using var output = File.Create(fullPath);
        await fileStream.CopyToAsync(output, ct);

        return storedName;
    }

    public Task<(Stream Stream, string ContentType)?> GetAsync(string fileName, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, fileName);

        if (!File.Exists(fullPath))
            return Task.FromResult<(Stream, string)?>(null);

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var contentType = extension == ".pdf" ? "application/pdf" : "text/plain";

        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult<(Stream, string)?>((stream, contentType));
    }

    public Task DeleteAsync(string fileName, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, fileName);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
