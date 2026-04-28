using Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

public class AuditLogTests
{
    [Fact]
    public void Create_ShouldInitializeSuccessAuditLog()
    {
        var log = AuditLog.Create(
            "user-1",
            "user@test.com",
            "Order.Create",
            "Order",
            "corr-1",
            "127.0.0.1");

        log.Status.Should().Be(AuditLogStatus.Success);
        log.Action.Should().Be("Order.Create");
        log.UserId.Should().Be("user-1");
    }

    [Fact]
    public void MarkAsFailed_ShouldSetFailedStatusAndMessage()
    {
        var log = AuditLog.Create(
            "user-1",
            "user@test.com",
            "Order.Create",
            "Order",
            "corr-1",
            "127.0.0.1");

        log.MarkAsFailed("boom");

        log.Status.Should().Be(AuditLogStatus.Failed);
        log.ErrorMessage.Should().Be("boom");
    }
}
