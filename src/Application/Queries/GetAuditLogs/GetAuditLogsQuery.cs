using Application.DTOs;
using Domain.Entities;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    string? UserId,
    string? Action,
    AuditLogStatus? Status,
    DateTime? From,
    DateTime? To,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<AuditLogDto>>>;
