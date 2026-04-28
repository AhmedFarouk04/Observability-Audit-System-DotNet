using Application.DTOs;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetMetricsLatency;

public sealed record GetMetricsLatencyQuery : IRequest<Result<MetricsLatencyDto>>;
