namespace Masa.Contrib.Data.UoW.EF;

public class UnitOfWorkManager : IUnitOfWorkManager
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkManager(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<IUnitOfWork> CreateDbContextAsync()
    {
        var scope = _serviceProvider.CreateAsyncScope();
        return Task.FromResult(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
    }

    public Task<IUnitOfWork> CreateDbContextAsync(MasaDbContextConfigurationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        if (string.IsNullOrEmpty(options.ConnectionString))
            throw new ArgumentException($"Invalid {nameof(options)}");

        var scope = _serviceProvider.CreateAsyncScope();
        var unitOfWorkAccessor = scope.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unitOfWorkAccessor.CurrentDbContextOptions = options;
        return Task.FromResult(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
    }
}
