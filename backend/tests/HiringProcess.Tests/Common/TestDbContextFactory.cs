using HiringProcess.Api.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HiringProcess.Tests.Common;

/// <summary>
/// Creates an isolated SQLite in-memory AppDbContext for each test.
/// Each call produces a fresh database - tests never share state.
/// The returned SqliteConnection must be disposed after the test to release the in-memory DB.
/// </summary>
public static class TestDbContextFactory
{
    public static (AppDbContext Db, SqliteConnection Connection) Create()
    {
        // Keep the connection open for the lifetime of the test - SQLite in-memory
        // databases are destroyed when the last connection closes.
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var db = new AppDbContext(options);
        db.Database.EnsureCreated();

        return (db, connection);
    }
}
