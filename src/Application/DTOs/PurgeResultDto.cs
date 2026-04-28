namespace Application.DTOs;

public class PurgeResultDto
{
    public int DeletedCount { get; init; }

    public DateTime OlderThanUtc { get; init; }
}
