namespace Masa.Contrib.Data.EntityFrameworkCore;

public abstract class MasaDbContextOptionsBuilder
{
    public DbContextOptionsBuilder DbContextOptionsBuilder;

    public IServiceProvider ServiceProvider { get; }

    internal bool EnableSoftDelete { get; private set; }

    protected MasaDbContextOptionsBuilder(IServiceProvider serviceProvider, DbContextOptions options, bool enableSoftDelete)
    {
        DbContextOptionsBuilder = new DbContextOptionsBuilder(options);
        ServiceProvider = serviceProvider;
        EnableSoftDelete = enableSoftDelete;
    }

    public MasaDbContextOptionsBuilder UseSoftDelete()
    {
        EnableSoftDelete = true;
        return this;
    }
}
