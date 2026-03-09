using HiringProcess.Api.Infrastructure.Email;

namespace HiringProcess.Tests.Common;

/// <summary>
/// No-op email service for tests — discards all messages silently.
/// </summary>
public sealed class NullEmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        => Task.CompletedTask;
}
