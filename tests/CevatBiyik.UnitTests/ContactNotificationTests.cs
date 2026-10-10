using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Email;
using Microsoft.Extensions.Configuration;

namespace CevatBiyik.UnitTests;

public sealed class ContactNotificationTests
{
    [Theory]
    [InlineData(true)] [InlineData(false)]
    public async Task Creation_QueuesAtomicallyOnlyWhenEnabled(bool enabled)
    {
        var repo = new FakeRepository<ContactMessage>();
        var service = new ContactService(repo, new CreateContactMessageValidator(), TimeProvider.System,
            new SmtpNotificationOptions { Enabled = enabled });
        var result = await service.CreateAsync(new CreateContactMessageDto { Name = "Test", Email = "test@example.test", Message = "Test message", Consent = true });
        Assert.Equal(enabled, Assert.Single(repo.Items).NotificationPending);
        Assert.Equal(1, repo.SaveCount);
        Assert.Equal(enabled ? "Bekliyor" : "Kapalı", result.NotificationStatus);
    }
    [Theory]
    [InlineData("test@example.test\r\nBcc:x@example.test")]
    [InlineData("Name <test@example.test>")]
    [InlineData("test@@example.test")]
    public async Task Email_RejectsHeaderInjectionOrDisplayNames(string email)
    {
        Assert.False((await new CreateContactMessageValidator().ValidateAsync(new CreateContactMessageDto
        { Name = "Test", Email = email, Message = "Message", Consent = true })).IsValid);
    }
    [Fact]
    public async Task Retry_OnlyExhaustedEnabledUnleasedJobsCanBeReset()
    {
        var repo = new FakeRepository<ContactMessage>();
        var row = new ContactMessage { Id = 1, NotificationPending = true, NotificationAttempts = 6 };
        repo.Seed(row);
        var service = new ContactService(repo, new CreateContactMessageValidator(), TimeProvider.System, new SmtpNotificationOptions { Enabled = true });
        Assert.StartsWith("Başarısız", (await service.GetAsync(1))!.NotificationStatus, StringComparison.Ordinal);
        row.NotificationLeaseUntil = DateTime.UtcNow.AddMinutes(1);
        await Assert.ThrowsAsync<CevatBiyik.Application.Exceptions.ConflictException>(() => service.RetryNotificationAsync(1));
        row.NotificationLeaseUntil = null;
        await service.RetryNotificationAsync(1); Assert.Equal(0, row.NotificationAttempts);
        await Assert.ThrowsAsync<CevatBiyik.Application.Exceptions.ConflictException>(() => service.RetryNotificationAsync(1));
        row.IsDeleted = true;
        await Assert.ThrowsAsync<CevatBiyik.Application.Exceptions.NotFoundException>(() => service.RetryNotificationAsync(1));
    }
    [Fact]
    public void Smtp_DefaultDisabledAndToStringDoesNotExposePassword()
    {
        var options = SmtpNotificationOptions.Read(new ConfigurationBuilder().Build());
        Assert.False(options.Enabled);
        options.Validate();
        var secret = new SmtpNotificationOptions { Password = "TEST_ONLY_SECRET" };
        Assert.DoesNotContain("TEST_ONLY_SECRET", secret.ToString(), StringComparison.Ordinal);
    }
    [Theory]
    [InlineData("bad host", 587, "from@example.test")]
    [InlineData("smtp.example.test", 25, "from@example.test")]
    [InlineData("smtp.example.test", 465, "from@example.test")]
    [InlineData("smtp.example.test", 587, "from@example.test\r\nBcc:x@example.test")]
    public void Smtp_InvalidOptionsFailWithGenericMessage(string host, int port, string from)
    {
        var options = new SmtpNotificationOptions { Enabled = true, Host = host, Port = port, From = from,
            Recipient = "admin@example.test", UserName = "test", Password = "TEST_ONLY_SECRET" };
        var exception = Assert.Throws<InvalidOperationException>(options.Validate);
        Assert.DoesNotContain("TEST_ONLY_SECRET", exception.Message, StringComparison.Ordinal);
    }
    [Fact]
    public void Mail_IsPlainTextWithFixedHeadersAndOnlyRecordIdentifier()
    {
        using var mail = SmtpContactEmailSender.CreateMessage(new SmtpNotificationOptions
        { Enabled = true, Host = "smtp.example.test", From = "from@example.test", Recipient = "admin@example.test", UserName = "test", Password = "secret" }, 42);
        Assert.False(mail.IsBodyHtml);
        Assert.Contains("42", mail.Body, StringComparison.Ordinal);
        Assert.Equal("admin@example.test", Assert.Single(mail.To).Address);
        Assert.Empty(mail.ReplyToList);
        Assert.DoesNotContain("secret", mail.Body, StringComparison.Ordinal);
    }
    [Theory]
    [InlineData(false, true)] [InlineData(true, false)]
    public async Task Worker_RecordsSuccessOrRetryWithoutPropagatingSmtpSecrets(bool fail, bool expected)
    {
        var store = new TestStore();
        var sender = new TestSender { Fail = fail };
        await ContactNotificationWorker.ProcessOneAsync(store, sender, CancellationToken.None);
        Assert.Equal(expected, store.Completed);
        Assert.Equal(1, sender.Calls);
    }
    [Fact]
    public async Task Worker_NoJobOrDeletedJobDoesNotSend()
    {
        var sender = new TestSender();
        await ContactNotificationWorker.ProcessOneAsync(new TestStore { HasJob = false }, sender, CancellationToken.None);
        await ContactNotificationWorker.ProcessOneAsync(new TestStore { Current = false }, sender, CancellationToken.None);
        Assert.Equal(0, sender.Calls);
    }
    [Fact]
    public async Task Worker_ShutdownLeavesLeaseForRecovery()
    {
        var store = new TestStore();
        using var ct = new CancellationTokenSource();
        ct.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => ContactNotificationWorker.ProcessOneAsync(store, new TestSender(), ct.Token));
        Assert.Null(store.Completed);
    }
    private sealed class TestStore : IContactNotificationStore
    {
        public bool HasJob { get; init; } = true;
        public bool Current { get; init; } = true;
        public bool? Completed { get; private set; }
        public Task<ContactNotificationJob?> ClaimAsync(CancellationToken ct) => Task.FromResult(HasJob ? new ContactNotificationJob(42, "lease", 1) : null);
        public Task<bool> IsCurrentAsync(ContactNotificationJob job, CancellationToken ct) => Task.FromResult(Current);
        public Task CompleteAsync(ContactNotificationJob job, bool sent, CancellationToken ct) { Completed = sent; return Task.CompletedTask; }
    }
    private sealed class TestSender : IContactEmailSender
    {
        public bool Fail { get; init; }
        public int Calls { get; private set; }
        public Task SendAsync(long id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested(); Calls++;
            return Fail ? throw new InvalidOperationException("TEST_ONLY_SMTP_SECRET") : Task.CompletedTask;
        }
    }
}
