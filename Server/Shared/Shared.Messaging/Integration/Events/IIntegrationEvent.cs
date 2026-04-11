namespace Shared.Messaging.Integration.Events;

public interface IIntegration
{
    Guid EventId => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName!;
}