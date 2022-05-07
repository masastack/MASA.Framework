namespace Masa.Contrib.Data.EntityFrameworkCore;

public class EFDbContextOptionsBuilder<TDbContext> : EFDbContextOptionsBuilder
    where TDbContext : MasaDbContext, IMasaDbContext
{
    public EFDbContextOptionsBuilder(IServiceProvider serviceProvider)
        : base(serviceProvider, new DbContextOptions<TDbContext>(), false)
    {
    }
}
