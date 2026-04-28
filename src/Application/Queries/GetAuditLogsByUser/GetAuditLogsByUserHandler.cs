using Application.DTOs;
using Domain.Repositories;
using Mapster;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetAuditLogsByUser;

public sealed class GetAuditLogsByUserHandler
    : IRequestHandler<GetAuditLogsByUserQuery, Result<IReadOnlyCollection<AuditLogDto>>>
{
    private readonly IAuditLogRepository _repository;

    public GetAuditLogsByUserHandler(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyCollection<AuditLogDto>>> Handle(
        GetAuditLogsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _repository.GetByUserIdAsync(request.UserId, request.Limit, cancellationToken);
        return Result<IReadOnlyCollection<AuditLogDto>>.Success(logs.Adapt<IReadOnlyCollection<AuditLogDto>>());
    }
}
