namespace MASA.BuildingBlocks.DDD.Domain.Repositories;
public class PaginatedList<TEntity>
    where TEntity : class, IEntity
{
    public long Total { get; set; }

    public int TotalPages { get; set; }

    public List<TEntity> Result { get; set; } = default!;
}
