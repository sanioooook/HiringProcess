namespace HiringProcess.Api.Infrastructure.FileStorage;

/// <summary>
/// Abstraction for file storage - swappable between local disk and cloud (S3, Azure Blob).
/// </summary>
public interface IFileStorageService
{
    /// <summary>Saves the uploaded stream and returns the stored file name.</summary>
    Task<string> SaveAsync(Stream fileStream, string originalFileName, CancellationToken ct = default);

    /// <summary>Returns the file stream for download, or null if not found.</summary>
    Task<(Stream Stream, string ContentType)?> GetAsync(string fileName, CancellationToken ct = default);

    /// <summary>Deletes a previously stored file.</summary>
    Task DeleteAsync(string fileName, CancellationToken ct = default);
}
