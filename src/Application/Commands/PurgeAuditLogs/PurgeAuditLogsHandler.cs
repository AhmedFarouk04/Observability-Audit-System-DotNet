using Application.DTOs;
using Domain.Repositories;
using MediatR;
using SharedKernel.Results;

namespace Application.Commands.PurgeAuditLogs;

public sealed class PurgeAuditLogsHandler : IRequestHandler<PurgeAuditLogsCommand, Result<PurgeResultDto>>
{
    private readonly IAuditLogRepository _repository;

    public PurgeAuditLogsHandler(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PurgeResultDto>> Handle(PurgeAuditLogsCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _repository.PurgeOlderThanAsync(request.OlderThanUtc, cancellationToken);

        return Result<PurgeResultDto>.Success(new PurgeResultDto
        {
            DeletedCount = deleted,
            OlderThanUtc = request.OlderThanUtc
        });
    }
}
