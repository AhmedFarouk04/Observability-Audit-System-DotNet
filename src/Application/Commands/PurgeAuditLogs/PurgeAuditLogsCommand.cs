using Application.DTOs;
using MediatR;
using SharedKernel.Results;

namespace Application.Commands.PurgeAuditLogs;

public sealed record PurgeAuditLogsCommand(DateTime OlderThanUtc) : IRequest<Result<PurgeResultDto>>;
