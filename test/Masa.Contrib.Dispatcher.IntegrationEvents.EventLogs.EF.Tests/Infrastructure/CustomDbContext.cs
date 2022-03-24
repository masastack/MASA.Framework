namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests.Infrastructure;

internal class CustomDbContext : MasaDbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {

    }
}
