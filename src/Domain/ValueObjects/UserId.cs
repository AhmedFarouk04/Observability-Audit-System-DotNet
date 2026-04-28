using SharedKernel.Exceptions;

namespace Domain.ValueObjects;

public sealed record UserId
{
    public UserId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("UserId cannot be empty.");
        }

        Value = value;
    }

    public string Value { get; }

    public static implicit operator string(UserId id)
    {
        return id.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
