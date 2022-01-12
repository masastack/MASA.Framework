namespace MASA.Contrib.DDD.Domain.Events;

public abstract record DomainQuery<TResult> : IDomainQuery<TResult>
    where TResult : notnull
{
    [JsonIgnore]
    public Guid Id { get; init; }

    [JsonIgnore]
    public DateTime CreationTime { get; init; }

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork
    {
        get => null;
        set => throw new NotSupportedException(nameof(UnitOfWork));
    }

    public abstract TResult Result { get; set; }

    public DomainQuery() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public DomainQuery(Guid id, DateTime creationTime)
    {
        this.Id = id;
        this.CreationTime = creationTime;
    }
}
