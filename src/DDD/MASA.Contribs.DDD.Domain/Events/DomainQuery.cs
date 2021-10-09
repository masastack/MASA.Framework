namespace MASA.Contribs.DDD.Domain.Events;

public abstract class DomainQuery<TResult> : IDomainQuery<TResult>
    where TResult : notnull
{
    public Guid Id { get; init; }

    public DateTime CreationTime { get; init; }

    public abstract TResult Result { get; set; }

    [JsonIgnore]
    public IUnitOfWork UnitOfWork { get; set; }

    public DomainQuery() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public DomainQuery(Guid id, DateTime creationTime)
    {
        Id = id;
        CreationTime = creationTime;
    }

    public override string ToString()
    {
        return $"Id:{Id}, CreationTime:{CreationTime}";
    }
}
