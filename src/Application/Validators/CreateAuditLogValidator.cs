using Application.Commands.CreateAuditLog;
using FluentValidation;

namespace Application.Validators;

public class CreateAuditLogValidator : AbstractValidator<CreateAuditLogCommand>
{
    public CreateAuditLogValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UserEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Action).NotEmpty().MaximumLength(200);
        RuleFor(x => x.EntityType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CorrelationId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IpAddress).NotEmpty().MaximumLength(50);
    }
}
