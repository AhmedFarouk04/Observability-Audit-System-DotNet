using Application.Queries.GetAuditLogs;
using FluentValidation;

namespace Application.Validators;

public class AuditLogFilterValidator : AbstractValidator<GetAuditLogsQuery>
{
    public AuditLogFilterValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 500);

        RuleFor(x => x)
            .Must(x => !x.From.HasValue || !x.To.HasValue || x.From <= x.To)
            .WithMessage("From date must be less than or equal to To date.");
    }
}
