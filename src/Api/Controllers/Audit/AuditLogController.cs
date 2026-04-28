using Application.Commands.PurgeAuditLogs;
using Application.DTOs;
using Application.Queries.ExportAuditLogs;
using Application.Queries.GetAuditLogById;
using Application.Queries.GetAuditLogs;
using Application.Queries.GetAuditLogsByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text;

namespace Api.Controllers.Audit;

[ApiController]
[Route("api/v1/audit-logs")]
[Produces("application/json")]
[EnableRateLimiting("api")]
public class AuditLogController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get paged audit logs with optional filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SharedKernel.Results.PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? userId,
        [FromQuery] string? action,
        [FromQuery] Domain.Entities.AuditLogStatus? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuditLogsQuery(userId, action, status, from, to, page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Get an audit log by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAuditLogByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Get audit logs for a specific user.
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUser(
        string userId,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAuditLogsByUserQuery(userId, limit), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Export audit logs as CSV.
    /// </summary>
    [HttpGet("export")]
    [Produces("text/csv")]
    public async Task<IActionResult> Export(
        [FromQuery] string? userId,
        [FromQuery] string? action,
        [FromQuery] Domain.Entities.AuditLogStatus? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ExportAuditLogsQuery(userId, action, status, from, to, 5000),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(result.Error ?? "Failed to export audit logs.");
        }

        var fileName = $"audit-logs-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";
        return File(Encoding.UTF8.GetBytes(result.Value), "text/csv", fileName);
    }

    /// <summary>
    /// Purge audit logs older than the provided UTC date.
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("purge")]
    [ProducesResponseType(typeof(PurgeResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Purge(
        [FromQuery] DateTime olderThan,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new PurgeAuditLogsCommand(olderThan.ToUniversalTime()), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
