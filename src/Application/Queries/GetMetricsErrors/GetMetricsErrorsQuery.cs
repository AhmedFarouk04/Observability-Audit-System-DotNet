using Application.DTOs;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetMetricsErrors;

public sealed record GetMetricsErrorsQuery : IRequest<Result<MetricsErrorsDto>>;
