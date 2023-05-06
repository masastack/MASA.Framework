// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;

public class IntegrationEventLog : IHasConcurrencyStamp
{
    public Guid Id { get; private set; }

    public Guid EventId { get; private set; }

    public string EventTypeName { get; private set; } = null!;

    [NotMapped]
    public string EventTypeShortName => EventTypeName.Split('.').Last();

    private object? _event;

    [NotMapped] public object Event => _event ??= JsonSerializer.Deserialize<object>(Content)!;

    [NotMapped] public IntegrationEventExpand? EventExpand { get; private set; }

    [NotMapped]
    public string Topic { get; private set; } = null!;

    public IntegrationEventStates State { get; set; } = IntegrationEventStates.NotPublished;

    public int TimesSent { get; set; } = 0;

    public DateTime CreationTime { get; private set; }

    public DateTime ModificationTime { get; set; }

    public string Content { get; private set; } = null!;

    public string ExpandContent { get; private set; } = string.Empty;

    public Guid TransactionId { get; private set; } = Guid.Empty;

    public string RowVersion { get; private set; }

    private IntegrationEventLog()
    {
        Id = Guid.NewGuid();
        Initialize();
    }

    public IntegrationEventLog(IIntegrationEvent @event, IntegrationEventExpand? eventExpand,  Guid transactionId) : this()
    {
        EventId = @event.GetEventId();
        CreationTime = @event.GetCreationTime();
        ModificationTime = @event.GetCreationTime();
        EventTypeName = @event.GetType().FullName!;
        Content = JsonSerializer.Serialize((object)@event);

        if (eventExpand != null)
        {
            ExpandContent = JsonSerializer.Serialize(eventExpand);
        }
        TransactionId = transactionId;
    }

    public void Initialize()
    {
        CreationTime = GetCurrentTime();
    }

    public virtual DateTime GetCurrentTime() => DateTime.UtcNow;

    public IntegrationEventLog DeserializeJsonContent()
    {
        var json = JsonSerializer.Deserialize<IntegrationEventTopic>(Content);
        Topic = json!.Topic;
        if (Topic.IsNullOrWhiteSpace())
        {
            Topic = EventTypeShortName;//Used to handle when the Topic is not persisted, it is consistent with the class name by default
        }
        if (!string.IsNullOrWhiteSpace(ExpandContent))
        {
            EventExpand = JsonSerializer.Deserialize<IntegrationEventExpand>(ExpandContent)!;
        }
        return this;
    }

    public void SetRowVersion(string rowVersion)
    {
        if (string.IsNullOrEmpty(rowVersion))
            throw new ArgumentNullException(nameof(rowVersion));

        RowVersion = rowVersion;
    }
}
