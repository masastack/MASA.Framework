namespace Masa.Contrib.Data.UoW.EF;

public class UnitOfWorkManager : IUnitOfWorkManager
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkManager(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<IUnitOfWork> CreateDbContextAsync(Masa.BuildingBlocks.Data.UoW.Options.MasaDbContextOptions dbContextOptions)
    {
        ArgumentNullException.ThrowIfNull(dbContextOptions, nameof(dbContextOptions));
        if (dbContextOptions.Connection == null && string.IsNullOrEmpty(dbContextOptions.ConnectionString))
            throw new ArgumentException($"Invalid {nameof(dbContextOptions)}");

        var scope = _serviceProvider.CreateAsyncScope();
        return Task.FromResult(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
    }
}
