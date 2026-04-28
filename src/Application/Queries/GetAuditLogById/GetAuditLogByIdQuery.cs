using Application.DTOs;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetAuditLogById;

public sealed record GetAuditLogByIdQuery(Guid Id) : IRequest<Result<AuditLogDto>>;
