namespace MASA.BuildingBlocks.Data.UoW;
public interface ITransaction
{
    [JsonIgnore]
    IUnitOfWork? UnitOfWork { get; set; }
}
