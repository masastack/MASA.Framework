namespace MASA.BuildingBlocks.Data.Contracts;
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
