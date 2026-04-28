using MediatR;
using SharedKernel.Results;

namespace Application.Commands.CreateAuditLog;

public sealed record CreateAuditLogCommand(
    string UserId,
    string UserEmail,
    string Action,
    string EntityType,
    string CorrelationId,
    string IpAddress,
    string? EntityId = null,
    string? OldValues = null,
    string? NewValues = null,
    string? UserAgent = null,
    long DurationMs = 0,
    bool IsFailure = false,
    string? ErrorMessage = null) : IRequest<Result<Guid>>;
