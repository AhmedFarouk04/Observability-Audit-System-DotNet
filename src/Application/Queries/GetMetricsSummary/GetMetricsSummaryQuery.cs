using Application.DTOs;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetMetricsSummary;

public sealed record GetMetricsSummaryQuery : IRequest<Result<MetricsSummaryDto>>;
