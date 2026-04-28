using System.Net;
using SharedKernel.Exceptions;

namespace Domain.ValueObjects;

public sealed record IpAddress
{
    public IpAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !IPAddress.TryParse(value, out _))
        {
            throw new DomainException("Invalid IP address.");
        }

        Value = value;
    }

    public string Value { get; }

    public static implicit operator string(IpAddress ip)
    {
        return ip.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
