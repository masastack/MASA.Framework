namespace Masa.Contrib.Data.EntityFrameworkCore.Filters;

public interface ISaveChangesFilter
{
    void OnExecuting(ChangeTracker changeTracker);
}
