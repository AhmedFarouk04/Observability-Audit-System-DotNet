using Application.Queries.GetAuditLogs;
using Application.Validators;
using FluentAssertions;

namespace Audit.UnitTests.Validators;

public class AuditLogFilterValidatorTests
{
    private readonly AuditLogFilterValidator _validator = new();

    [Fact]
    public void Validate_WhenPageIsInvalid_ShouldFail()
    {
        var query = new GetAuditLogsQuery(null, null, null, null, null, 0, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenDateRangeInvalid_ShouldFail()
    {
        var query = new GetAuditLogsQuery(
            null,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(-1),
            1,
            20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }
}
