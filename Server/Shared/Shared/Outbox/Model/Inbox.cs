using Shared.DDD;

namespace Shared.Outbox.Model;

public sealed class Inbox : Entity<long>
{
    public Guid MessageId { get; private set; }
    public DateTime ReceivedOn { get; private set; }

    private Inbox() {}

    private Inbox(Guid messageId)
    {
        MessageId = messageId;
        ReceivedOn = DateTime.UtcNow;
    }

    public static Inbox Create(Guid messageId)
    {
        return new Inbox(messageId);
    }
}
