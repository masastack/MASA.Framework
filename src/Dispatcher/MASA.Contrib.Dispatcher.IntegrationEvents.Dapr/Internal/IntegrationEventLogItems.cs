namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Internal;

/// <summary>
/// Use the local queue to retry sending failed messages
/// </summary>
internal class IntegrationEventLogItems
{
    public Guid EventId { get; }

    public string Topic { get; }

    public DateTime CreationTime { get; }

    public object Event { get; }

    public IntegrationEventLogItems(Guid eventId, string topic,object @event)
    {
        EventId = eventId;
        Topic = topic;
        CreationTime = DateTime.UtcNow;
        Event = @event;
    }
}
