namespace MASA.Contrib.DDD.Domain.Events;

public abstract record DomainQuery<TResult>(Guid Id, DateTime CreationTime) : IDomainQuery<TResult>
    where TResult : notnull
{
    [JsonIgnore]
    public IUnitOfWork? UnitOfWork
    {
        get => null;
        set => throw new NotSupportedException(nameof(UnitOfWork));
    }

    public abstract TResult Result { get; set; }

    public DomainQuery() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
