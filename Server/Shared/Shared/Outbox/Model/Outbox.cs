namespace Shared.Outbox.Model;

public sealed class Outbox
{
    public long Id {get; private set;}
    public string Type { get; private set; } = default!;
    public string Payload { get; private set; } = default!;
    public char Status { get; private set; } = default;
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }
}