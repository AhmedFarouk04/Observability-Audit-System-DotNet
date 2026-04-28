using Application.DTOs;
using Domain.Entities;
using Domain.Repositories;
using Mapster;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetAuditLogs;

public sealed class GetAuditLogsHandler : IRequestHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
{
    private readonly IAuditLogRepository _repository;

    public GetAuditLogsHandler(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var filter = new AuditLogFilter
        {
            UserId = request.UserId,
            Action = request.Action,
            Status = request.Status,
            From = request.From,
            To = request.To,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var paged = await _repository.GetPagedAsync(filter, cancellationToken);

        var result = new PagedResult<AuditLogDto>
        {
            Items = paged.Items.Adapt<IEnumerable<AuditLogDto>>(),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };

        return Result<PagedResult<AuditLogDto>>.Success(result);
    }
}
