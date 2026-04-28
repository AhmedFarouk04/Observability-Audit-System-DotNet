using Application.DTOs;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetAuditLogsByUser;

public sealed record GetAuditLogsByUserQuery(string UserId, int Limit = 50)
    : IRequest<Result<IReadOnlyCollection<AuditLogDto>>>;
