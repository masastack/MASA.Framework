namespace MASA.Contrib.Data.Uow.EF;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseUoW<TDbContext>(
        this IDispatcherOptions options,
        Action<MasaDbContextOptionsBuilder>? optionsBuilder = null)
        where TDbContext : MasaDbContext
    {
        options.Services.AddUoW<TDbContext>(optionsBuilder);
        return options;
    }
}
