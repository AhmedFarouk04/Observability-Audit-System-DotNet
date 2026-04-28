using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Results;

namespace Application.Commands.CreateAuditLog;

public sealed class CreateAuditLogHandler : IRequestHandler<CreateAuditLogCommand, Result<Guid>>
{
    private readonly IAuditLogRepository _repository;
    private readonly ILogger<CreateAuditLogHandler> _logger;

    public CreateAuditLogHandler(IAuditLogRepository repository, ILogger<CreateAuditLogHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
    {
        var auditLog = AuditLog.Create(
            request.UserId,
            request.UserEmail,
            request.Action,
            request.EntityType,
            request.CorrelationId,
            request.IpAddress,
            request.EntityId,
            request.OldValues,
            request.NewValues,
            request.UserAgent);

        if (request.IsFailure)
        {
            auditLog.MarkAsFailed(request.ErrorMessage ?? "Unknown error");
        }

        auditLog.SetDuration(request.DurationMs);

        await _repository.AddAsync(auditLog, cancellationToken);

        _logger.LogInformation(
            "Audit log created for action {Action} by user {UserId} with CorrelationId {CorrelationId}",
            request.Action,
            request.UserId,
            request.CorrelationId);

        return Result<Guid>.Success(auditLog.Id);
    }
}
