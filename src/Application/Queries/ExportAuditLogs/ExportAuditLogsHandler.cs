using Domain.Entities;
using Domain.Repositories;
using MediatR;
using SharedKernel.Results;
using System.Text;

namespace Application.Queries.ExportAuditLogs;

public sealed class ExportAuditLogsHandler : IRequestHandler<ExportAuditLogsQuery, Result<string>>
{
    private readonly IAuditLogRepository _repository;

    public ExportAuditLogsHandler(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<string>> Handle(ExportAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var filter = new AuditLogFilter
        {
            UserId = request.UserId,
            Action = request.Action,
            Status = request.Status,
            From = request.From,
            To = request.To,
            Page = 1,
            PageSize = Math.Clamp(request.Limit, 1, 20_000)
        };

        var paged = await _repository.GetPagedAsync(filter, cancellationToken);
        var csv = BuildCsv(paged.Items);

        return Result<string>.Success(csv);
    }

    private static string BuildCsv(IEnumerable<AuditLog> logs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,UserId,UserEmail,Action,EntityType,EntityId,Status,DurationMs,Timestamp,CorrelationId,IpAddress,ErrorMessage");

        foreach (var log in logs)
        {
            sb.AppendLine(string.Join(',',
                Escape(log.Id.ToString()),
                Escape(log.UserId),
                Escape(log.UserEmail),
                Escape(log.Action),
                Escape(log.EntityType),
                Escape(log.EntityId),
                Escape(log.Status.ToString()),
                Escape(log.DurationMs.ToString()),
                Escape(log.Timestamp.ToString("O")),
                Escape(log.CorrelationId),
                Escape(log.IpAddress),
                Escape(log.ErrorMessage)));
        }

        return sb.ToString();
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}
