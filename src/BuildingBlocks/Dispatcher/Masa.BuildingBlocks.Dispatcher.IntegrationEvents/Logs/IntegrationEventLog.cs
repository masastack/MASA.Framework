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

    [NotMapped]
    public IIntegrationEvent Event { get; private set; } = null!;

    public IntegrationEventStates State { get; set; } = IntegrationEventStates.NotPublished;

    public int TimesSent { get; set; } = 0;

    public DateTime CreationTime { get; private set; }

    public DateTime ModificationTime { get; set; }

    public string Content { get; private set; } = null!;

    public Guid TransactionId { get; private set; } = Guid.Empty;

    public string RowVersion { get; private set; }

    private IntegrationEventLog()
    {
        Id = Guid.NewGuid();
        Initialize();
    }

    public IntegrationEventLog(IIntegrationEvent @event, Guid transactionId) : this()
    {
        EventId = @event.GetEventId();
        CreationTime = @event.GetCreationTime();
        ModificationTime = @event.GetCreationTime();
        EventTypeName = @event.GetType().FullName!;
        Content = System.Text.Json.JsonSerializer.Serialize((object)@event);
        TransactionId = transactionId;
    }

    public void Initialize()
    {
        this.CreationTime = this.GetCurrentTime();
    }

    public virtual DateTime GetCurrentTime() => DateTime.UtcNow;

    public IntegrationEventLog DeserializeJsonContent(Type type)
    {
        Event = (System.Text.Json.JsonSerializer.Deserialize(Content, type) as IIntegrationEvent)!;
        Event?.SetEventId(this.EventId);
        Event?.SetCreationTime(this.CreationTime);
        return this;
    }

    public void SetRowVersion(string rowVersion)
    {
        if (string.IsNullOrEmpty(rowVersion))
            throw new ArgumentNullException(nameof(rowVersion));

        RowVersion = rowVersion;
    }
}
