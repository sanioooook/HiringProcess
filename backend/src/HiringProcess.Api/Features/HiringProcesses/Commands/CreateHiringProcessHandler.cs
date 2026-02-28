using FluentValidation;
using HiringProcess.Api.Common;
using HiringProcess.Api.Features.HiringProcesses.Models;
using HiringProcess.Api.Infrastructure;

namespace HiringProcess.Api.Features.HiringProcesses.Commands;

/// <summary>
/// Creates a new HiringProcess record owned by the authenticated user.
/// </summary>
public sealed class CreateHiringProcessHandler
{
    private readonly AppDbContext _db;
    private readonly IValidator<CreateHiringProcessCommand> _validator;

    public CreateHiringProcessHandler(AppDbContext db, IValidator<CreateHiringProcessCommand> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<HiringProcessDto>> HandleAsync(
        CreateHiringProcessCommand command,
        CancellationToken ct = default)
    {
        // Validate
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        // Build entity
        var now = DateTime.UtcNow;
        var entity = new HiringProcessEntity
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            CompanyName = command.CompanyName.Trim(),
            ContactChannel = command.ContactChannel.Trim(),
            ContactPerson = command.ContactPerson?.Trim(),
            FirstContactDate = command.FirstContactDate,
            LastContactDate = command.LastContactDate,
            VacancyPublishedDate = command.VacancyPublishedDate,
            ApplicationDate = command.ApplicationDate,
            AppliedWith = command.AppliedWith?.Trim(),
            AppliedLink = command.AppliedLink?.Trim(),
            CoverLetter = command.CoverLetter,
            SalaryRange = command.SalaryRange?.Trim(),
            CurrentStage = command.CurrentStage?.Trim(),
            VacancyLink = command.VacancyLink?.Trim(),
            VacancyText = command.VacancyText,
            Notes = command.Notes,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assign stages via the computed property (handles serialization)
        entity.HiringStages = command.HiringStages ?? [];

        // Persist
        _db.HiringProcesses.Add(entity);
        await _db.SaveChangesAsync(ct);

        return HiringProcessMapper.ToDto(entity);
    }
}
