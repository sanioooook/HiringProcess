using FluentValidation;
using HiringProcess.Api.Common;
using HiringProcess.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Features.HiringProcesses.Commands;

/// <summary>
/// Updates all mutable fields of an existing hiring process.
/// Returns NotFound if the record does not exist or belongs to another user.
/// </summary>
public sealed class UpdateHiringProcessHandler
{
    private readonly AppDbContext _db;
    private readonly IValidator<UpdateHiringProcessCommand> _validator;

    public UpdateHiringProcessHandler(AppDbContext db, IValidator<UpdateHiringProcessCommand> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Result<HiringProcessDto>> HandleAsync(
        UpdateHiringProcessCommand command,
        CancellationToken ct = default)
    {
        // Validate
        var validation = await _validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        // Load entity - ownership enforced here
        var entity = await _db.HiringProcesses
            .FirstOrDefaultAsync(h => h.Id == command.Id && h.UserId == command.UserId, ct);

        if (entity is null)
            return Error.NotFound;

        // Apply changes
        entity.CompanyName = command.CompanyName.Trim();
        entity.ContactChannel = command.ContactChannel.Trim();
        entity.ContactPerson = command.ContactPerson?.Trim();
        entity.FirstContactDate = command.FirstContactDate;
        entity.LastContactDate = command.LastContactDate;
        entity.VacancyPublishedDate = command.VacancyPublishedDate;
        entity.ApplicationDate = command.ApplicationDate;
        entity.AppliedWith = command.AppliedWith?.Trim();
        entity.AppliedLink = command.AppliedLink?.Trim();
        entity.CoverLetter = command.CoverLetter;
        entity.SalaryRange = command.SalaryRange?.Trim();
        entity.CurrentStage = command.CurrentStage?.Trim();
        entity.VacancyLink = command.VacancyLink?.Trim();
        entity.VacancyText = command.VacancyText;
        entity.Notes = command.Notes;
        entity.HiringStages = command.HiringStages ?? [];
        entity.UpdatedAt = DateTime.UtcNow;

        // Persist
        await _db.SaveChangesAsync(ct);

        return HiringProcessMapper.ToDto(entity);
    }
}
