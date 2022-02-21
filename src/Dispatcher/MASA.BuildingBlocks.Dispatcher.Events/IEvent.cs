namespace MASA.BuildingBlocks.Dispatcher.Events;

public interface IEvent
{
    [JsonIgnore]
    Guid Id { get; }

    [JsonIgnore]
    DateTime CreationTime { get; }
}

public interface IEvent<TResult> : IEvent
    where TResult : notnull
{
    TResult Result { get; set; }
}
