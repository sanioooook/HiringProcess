using HiringProcess.Api.Features.Auth.Models;
using HiringProcess.Api.Features.HiringProcesses.Models;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Api.Infrastructure;

/// <summary>
/// Single EF Core DbContext for the application.
/// Used exclusively inside command/query handlers — never injected into controllers.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<HiringProcessEntity> HiringProcesses => Set<HiringProcessEntity>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureHiringProcesses(modelBuilder);
        ConfigureRefreshTokens(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                  .ValueGeneratedOnAdd();

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(320);

            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.DisplayName)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(u => u.PasswordHash)
                  .HasMaxLength(512);

            entity.Property(u => u.GoogleId)
                  .HasMaxLength(128);

            entity.HasIndex(u => u.GoogleId)
                  .IsUnique()
                  .HasFilter("\"GoogleId\" IS NOT NULL");

            entity.Property(u => u.CreatedAt)
                  .IsRequired();

            entity.Property(u => u.Language)
                  .IsRequired()
                  .HasMaxLength(8)
                  .HasDefaultValue("en");
        });
    }

    private static void ConfigureHiringProcesses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HiringProcessEntity>(entity =>
        {
            entity.ToTable("hiring_processes");
            entity.HasKey(h => h.Id);

            entity.Property(h => h.Id)
                  .ValueGeneratedOnAdd();

            entity.Property(h => h.UserId)
                  .IsRequired();

            entity.HasIndex(h => h.UserId);

            // Required string fields
            entity.Property(h => h.CompanyName)
                  .IsRequired()
                  .HasMaxLength(500);

            entity.Property(h => h.ContactChannel)
                  .IsRequired()
                  .HasMaxLength(200);

            // Optional string fields
            entity.Property(h => h.ContactPerson).HasMaxLength(300);
            entity.Property(h => h.AppliedWith).HasMaxLength(500);
            entity.Property(h => h.AppliedLink).HasMaxLength(2048);
            entity.Property(h => h.SalaryRange).HasMaxLength(200);
            entity.Property(h => h.CurrentStage).HasMaxLength(200);
            entity.Property(h => h.VacancyLink).HasMaxLength(2048);
            entity.Property(h => h.VacancyFileName).HasMaxLength(500);

            // Unlimited-length text fields
            entity.Property(h => h.CoverLetter);
            entity.Property(h => h.VacancyText);
            entity.Property(h => h.Notes);

            // Stages stored as pipe-delimited raw string
            entity.Property(h => h.HiringStagesRaw)
                  .HasColumnName("hiring_stages")
                  .HasDefaultValue(string.Empty);

            // HiringStages is a computed property — exclude from mapping
            entity.Ignore(h => h.HiringStages);

            entity.Property(h => h.CreatedAt).IsRequired();
            entity.Property(h => h.UpdatedAt).IsRequired();
        });
    }

    private static void ConfigureRefreshTokens(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id).ValueGeneratedNever();

            entity.Property(r => r.Token)
                  .IsRequired()
                  .HasMaxLength(128);

            entity.HasIndex(r => r.Token).IsUnique();
            entity.HasIndex(r => r.UserId);

            entity.Property(r => r.ExpiresAt).IsRequired();
            entity.Property(r => r.CreatedAt).IsRequired();
        });
    }
}
