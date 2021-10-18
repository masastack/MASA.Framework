namespace MASA.Contrib.ReadWriteSpliting.CQRS.Queries;

public abstract record Query<TResult> : IQuery<TResult>
    where TResult : notnull
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    public abstract TResult Result { get; set; }

    public Query() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Query(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
