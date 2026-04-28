using Domain.Entities;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.ExportAuditLogs;

public sealed record ExportAuditLogsQuery(
    string? UserId,
    string? Action,
    AuditLogStatus? Status,
    DateTime? From,
    DateTime? To,
    int Limit = 5000) : IRequest<Result<string>>;
