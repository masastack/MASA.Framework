namespace MASA.Contrib.ReadWriteSpliting.CQRS.Queries;

public abstract record Query<TResult>(Guid Id, DateTime CreationTime) : IQuery<TResult>
    where TResult : notnull
{
    [JsonIgnore]
    public Guid Id { get; } = Id;

    [JsonIgnore]
    public DateTime CreationTime { get; } = CreationTime;

    public abstract TResult Result { get; set; }

    public Query() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
