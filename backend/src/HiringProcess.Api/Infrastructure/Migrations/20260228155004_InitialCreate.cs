using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiringProcess.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hiring_processes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContactChannel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    FirstContactDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LastContactDate = table.Column<DateOnly>(type: "date", nullable: true),
                    VacancyPublishedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ApplicationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AppliedWith = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AppliedLink = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CoverLetter = table.Column<string>(type: "text", nullable: true),
                    SalaryRange = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    hiring_stages = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    CurrentStage = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VacancyLink = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    VacancyFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VacancyText = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hiring_processes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    GoogleId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hiring_processes_UserId",
                table: "hiring_processes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_GoogleId",
                table: "users",
                column: "GoogleId",
                unique: true,
                filter: "\"GoogleId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hiring_processes");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
