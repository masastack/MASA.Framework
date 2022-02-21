namespace MASA.BuildingBlocks.DDD.Domain.Services;
public interface IDomainService
{
    IDomainEventBus EventBus { get; }
}