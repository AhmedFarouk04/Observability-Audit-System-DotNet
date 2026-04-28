using Application.Queries.GetMetricsErrors;
using Application.Queries.GetMetricsLatency;
using Application.Queries.GetMetricsSummary;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers.Metrics;

[ApiController]
[Route("api/v1/metrics")]
[EnableRateLimiting("api")]
public class MetricsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MetricsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMetricsSummaryQuery(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("errors")]
    public async Task<IActionResult> GetErrors(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMetricsErrorsQuery(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("latency")]
    public async Task<IActionResult> GetLatency(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMetricsLatencyQuery(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
