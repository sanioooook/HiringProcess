using HiringProcess.Api.Features.HiringProcesses.Commands;
using HiringProcess.Api.Features.HiringProcesses.Models;
using HiringProcess.Api.Infrastructure.FileStorage;
using HiringProcess.Tests.Common;
using Moq;

namespace HiringProcess.Tests.Features.HiringProcesses;

/// <summary>
/// Tests for DeleteHiringProcessHandler. Uses a mock IFileStorageService to avoid disk I/O.
/// </summary>
public sealed class DeleteHiringProcessHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SqliteConnection _connection;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly DeleteHiringProcessHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public DeleteHiringProcessHandlerTests()
    {
        (_db, _connection) = TestDbContextFactory.Create();
        _fileStorageMock = new Mock<IFileStorageService>();
        _handler = new DeleteHiringProcessHandler(_db, _fileStorageMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ExistingRecord_DeletesFromDb()
    {
        // Arrange
        var entity = await SeedEntityAsync();

        // Act
        var result = await _handler.HandleAsync(new DeleteHiringProcessCommand(entity.Id, _userId));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(_db.HiringProcesses.Find(entity.Id));
    }

    [Fact]
    public async Task HandleAsync_RecordWithFile_DeletesFileFromStorage()
    {
        // Arrange
        var entity = await SeedEntityAsync(fileName: "vacancy.pdf");

        // Act
        await _handler.HandleAsync(new DeleteHiringProcessCommand(entity.Id, _userId));

        // Assert
        _fileStorageMock.Verify(s => s.DeleteAsync("vacancy.pdf", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_RecordWithNoFile_DoesNotCallFileStorage()
    {
        // Arrange
        var entity = await SeedEntityAsync(fileName: null);

        // Act
        await _handler.HandleAsync(new DeleteHiringProcessCommand(entity.Id, _userId));

        // Assert
        _fileStorageMock.Verify(
            s => s.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WrongUserId_ReturnsNotFound()
    {
        // Arrange
        var entity = await SeedEntityAsync();

        // Act
        var result = await _handler.HandleAsync(new DeleteHiringProcessCommand(entity.Id, Guid.NewGuid()));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NotFound", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_NonExistentId_ReturnsNotFound()
    {
        // Act
        var result = await _handler.HandleAsync(new DeleteHiringProcessCommand(Guid.NewGuid(), _userId));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NotFound", result.Error.Code);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private async Task<HiringProcessEntity> SeedEntityAsync(string? fileName = null)
    {
        var entity = new HiringProcessEntity
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            CompanyName = "Test Corp",
            ContactChannel = "Email",
            HiringStagesRaw = string.Empty,
            VacancyFileName = fileName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.HiringProcesses.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}
