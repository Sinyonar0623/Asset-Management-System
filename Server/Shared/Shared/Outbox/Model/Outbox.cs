using Shared.DDD;

namespace Shared.Outbox.Model;

public sealed class Outbox : Entity<long>
{
    public const char PendingStatus = 'P';
    public const char ProcessedStatus = 'S';
    public const char FailedStatus = 'F';

    public string Type { get; private set; } = default!;
    public string Payload { get; private set; } = default!;
    public char Status { get; private set; } = default;
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }

    private Outbox() {}

    private Outbox(string type, string payload, DateTime occurredOn)
    {
        Type = type;
        Payload = payload;
        Status = PendingStatus;
        OccurredOn = occurredOn;
        RetryCount = 0;
    }

    public static Outbox Create(string type, string payload, DateTime? occurredOn = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);

        return new Outbox(type, payload, occurredOn ?? DateTime.UtcNow);
    }

    public void MarkProcessed(DateTime? processedOn = null)
    {
        Status = ProcessedStatus;
        ProcessedOn = processedOn ?? DateTime.UtcNow;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);

        Status = FailedStatus;
        RetryCount++;
        Error = error;
        ProcessedOn = null;
    }

    public void ResetForRetry()
    {
        Status = PendingStatus;
        ProcessedOn = null;
        Error = null;
    }
}
