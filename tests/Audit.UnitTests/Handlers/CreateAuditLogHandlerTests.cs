using Application.Commands.CreateAuditLog;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Audit.UnitTests.Handlers;

public class CreateAuditLogHandlerTests
{
    private readonly Mock<IAuditLogRepository> _repositoryMock = new();
    private readonly Mock<ILogger<CreateAuditLogHandler>> _loggerMock = new();

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        var handler = new CreateAuditLogHandler(_repositoryMock.Object, _loggerMock.Object);
        var command = new CreateAuditLogCommand(
            "user-1",
            "user@test.com",
            "User.Login",
            "User",
            "corr-123",
            "127.0.0.1");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
