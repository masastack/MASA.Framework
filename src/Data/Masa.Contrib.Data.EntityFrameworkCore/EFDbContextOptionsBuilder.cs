namespace Masa.Contrib.Data.EntityFrameworkCore;

public abstract class EFDbContextOptionsBuilder
{
    public readonly DbContextOptionsBuilder DbContextOptionsBuilder;

    public IServiceProvider ServiceProvider { get; }

    internal bool EnableSoftDelete { get; private set; }

    protected EFDbContextOptionsBuilder(IServiceProvider serviceProvider, DbContextOptions options, bool enableSoftDelete)
    {
        DbContextOptionsBuilder = new DbContextOptionsBuilder(options);
        ServiceProvider = serviceProvider;
        EnableSoftDelete = enableSoftDelete;
    }

    public EFDbContextOptionsBuilder UseSoftDelete()
    {
        EnableSoftDelete = true;
        return this;
    }
}
