using SharedKernel.Exceptions;

namespace Domain.ValueObjects;

public sealed record CorrelationId
{
    public CorrelationId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("CorrelationId cannot be empty.");
        }

        Value = value;
    }

    public string Value { get; }

    public static CorrelationId New()
    {
        return new CorrelationId(Guid.NewGuid().ToString());
    }

    public static implicit operator string(CorrelationId id)
    {
        return id.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
