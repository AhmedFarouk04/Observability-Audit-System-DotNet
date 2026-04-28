using Application.DTOs;
using Domain.Repositories;
using Mapster;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetAuditLogById;

public sealed class GetAuditLogByIdHandler : IRequestHandler<GetAuditLogByIdQuery, Result<AuditLogDto>>
{
    private readonly IAuditLogRepository _repository;

    public GetAuditLogByIdHandler(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AuditLogDto>> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
    {
        var auditLog = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (auditLog is null)
        {
            return Result<AuditLogDto>.Failure("Audit log was not found.");
        }

        return Result<AuditLogDto>.Success(auditLog.Adapt<AuditLogDto>());
    }
}
