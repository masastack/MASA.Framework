namespace Masa.Contrib.Data.UoW.EF;

public class UnitOfWorkManager : IUnitOfWorkManager
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkManager(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public IUnitOfWork CreateDbContext()
    {
        var scope = _serviceProvider.CreateAsyncScope();
        return scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    public IUnitOfWork CreateDbContext(MasaDbContextConfigurationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        if (string.IsNullOrEmpty(options.ConnectionString))
            throw new ArgumentException($"Invalid {nameof(options)}");

        var scope = _serviceProvider.CreateAsyncScope();
        var unitOfWorkAccessor = scope.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unitOfWorkAccessor.CurrentDbContextOptions = options;
        return scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }
}
