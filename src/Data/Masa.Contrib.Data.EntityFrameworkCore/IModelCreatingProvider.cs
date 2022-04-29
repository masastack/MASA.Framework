namespace Masa.Contrib.Data.EntityFrameworkCore;

public interface IModelCreatingProvider
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="modelBuilder"></param>
    void Configure(ModelBuilder modelBuilder);
}
