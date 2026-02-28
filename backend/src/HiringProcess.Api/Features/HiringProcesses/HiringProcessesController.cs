using System.Security.Claims;
using HiringProcess.Api.Common;
using HiringProcess.Api.Common.Extensions;
using HiringProcess.Api.Features.HiringProcesses.Commands;
using HiringProcess.Api.Features.HiringProcesses.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiringProcess.Api.Features.HiringProcesses;

/// <summary>
/// HTTP adapter for all hiring-process CRUD and file operations.
/// Extracts the authenticated user ID from JWT claims; all ownership enforcement happens in handlers.
/// </summary>
[ApiController]
[Route("api/hiring-processes")]
[Authorize]
public sealed class HiringProcessesController : ControllerBase
{
    private readonly GetHiringProcessesHandler _getListHandler;
    private readonly GetHiringProcessByIdHandler _getByIdHandler;
    private readonly CreateHiringProcessHandler _createHandler;
    private readonly UpdateHiringProcessHandler _updateHandler;
    private readonly DeleteHiringProcessHandler _deleteHandler;
    private readonly UploadVacancyFileHandler _uploadFileHandler;
    private readonly DownloadVacancyFileHandler _downloadFileHandler;

    public HiringProcessesController(
        GetHiringProcessesHandler getListHandler,
        GetHiringProcessByIdHandler getByIdHandler,
        CreateHiringProcessHandler createHandler,
        UpdateHiringProcessHandler updateHandler,
        DeleteHiringProcessHandler deleteHandler,
        UploadVacancyFileHandler uploadFileHandler,
        DownloadVacancyFileHandler downloadFileHandler)
    {
        _getListHandler = getListHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _uploadFileHandler = uploadFileHandler;
        _downloadFileHandler = downloadFileHandler;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID claim not found."));

    // GET /api/hiring-processes
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<HiringProcessDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? currentStage = null,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] string sortDirection = "desc",
        CancellationToken ct = default)
    {
        var query = new GetHiringProcessesQuery(
            CurrentUserId, page, pageSize, search, currentStage, sortBy, sortDirection);

        var result = await _getListHandler.HandleAsync(query, ct);
        return result.ToActionResult();
    }

    // GET /api/hiring-processes/{id}
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(HiringProcessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetHiringProcessByIdQuery(id, CurrentUserId);
        var result = await _getByIdHandler.HandleAsync(query, ct);
        return result.ToActionResult();
    }

    // POST /api/hiring-processes
    [HttpPost]
    [ProducesResponseType(typeof(HiringProcessDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateHiringProcessRequest request,
        CancellationToken ct)
    {
        var command = request.ToCommand(CurrentUserId);
        var result = await _createHandler.HandleAsync(command, ct);

        if (result.IsFailure)
            return result.ToActionResult();

        return result.ToCreatedResult(nameof(GetById), new { id = result.Value!.Id });
    }

    // PUT /api/hiring-processes/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(HiringProcessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateHiringProcessRequest request,
        CancellationToken ct)
    {
        var command = request.ToCommand(id, CurrentUserId);
        var result = await _updateHandler.HandleAsync(command, ct);
        return result.ToActionResult();
    }

    // DELETE /api/hiring-processes/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new DeleteHiringProcessCommand(id, CurrentUserId);
        var result = await _deleteHandler.HandleAsync(command, ct);
        return result.ToActionResult();
    }

    // POST /api/hiring-processes/{id}/file
    [HttpPost("{id:guid}/file")]
    [ProducesResponseType(typeof(UploadVacancyFileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> UploadFile(
        Guid id,
        IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ProblemDetail("Validation", "File is required and must not be empty."));

        await using var stream = file.OpenReadStream();

        var command = new UploadVacancyFileCommand(id, CurrentUserId, stream, file.FileName);
        var result = await _uploadFileHandler.HandleAsync(command, ct);
        return result.ToActionResult();
    }

    // GET /api/hiring-processes/{id}/file
    [HttpGet("{id:guid}/file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadFile(Guid id, CancellationToken ct)
    {
        var query = new DownloadVacancyFileQuery(id, CurrentUserId);
        var result = await _downloadFileHandler.HandleAsync(query, ct);

        if (result.IsFailure)
            return result.ToActionResult();

        var file = result.Value!;
        return File(file.FileStream, file.ContentType, file.FileName);
    }
}

// Request DTOs (separate from command records to decouple HTTP body
// shape from internal handler contracts)

/// <summary>HTTP request body for creating a hiring process.</summary>
public sealed record CreateHiringProcessRequest(
    string CompanyName,
    string ContactChannel,
    string? ContactPerson,
    DateOnly? FirstContactDate,
    DateOnly? LastContactDate,
    DateOnly? VacancyPublishedDate,
    DateOnly? ApplicationDate,
    string? AppliedWith,
    string? AppliedLink,
    string? CoverLetter,
    string? SalaryRange,
    List<string>? HiringStages,
    string? CurrentStage,
    string? VacancyLink,
    string? VacancyText,
    string? Notes)
{
    public CreateHiringProcessCommand ToCommand(Guid userId) => new(
        userId, CompanyName, ContactChannel, ContactPerson,
        FirstContactDate, LastContactDate, VacancyPublishedDate, ApplicationDate,
        AppliedWith, AppliedLink, CoverLetter, SalaryRange,
        HiringStages, CurrentStage, VacancyLink, VacancyText, Notes);
}

/// <summary>HTTP request body for updating a hiring process.</summary>
public sealed record UpdateHiringProcessRequest(
    string CompanyName,
    string ContactChannel,
    string? ContactPerson,
    DateOnly? FirstContactDate,
    DateOnly? LastContactDate,
    DateOnly? VacancyPublishedDate,
    DateOnly? ApplicationDate,
    string? AppliedWith,
    string? AppliedLink,
    string? CoverLetter,
    string? SalaryRange,
    List<string>? HiringStages,
    string? CurrentStage,
    string? VacancyLink,
    string? VacancyText,
    string? Notes)
{
    public UpdateHiringProcessCommand ToCommand(Guid id, Guid userId) => new(
        id, userId, CompanyName, ContactChannel, ContactPerson,
        FirstContactDate, LastContactDate, VacancyPublishedDate, ApplicationDate,
        AppliedWith, AppliedLink, CoverLetter, SalaryRange,
        HiringStages, CurrentStage, VacancyLink, VacancyText, Notes);
}
