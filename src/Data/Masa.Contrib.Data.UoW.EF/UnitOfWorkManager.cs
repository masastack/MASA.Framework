﻿namespace Masa.Contrib.Data.UoW.EF;

public class UnitOfWorkManager<TDbContext> : IUnitOfWorkManager where TDbContext : MasaDbContext
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkManager(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    /// <summary>
    ///
    /// </summary>
    /// <param name="lazyLoading">Deferred creation of DbContext, easy to specify tenant or environment by yourself, which is very effective for physical isolation</param>
    /// <returns></returns>
    public IUnitOfWork CreateDbContext(bool lazyLoading = true)
    {
        var scope = _serviceProvider.CreateAsyncScope();
        if (!lazyLoading)
            scope.ServiceProvider.GetRequiredService<TDbContext>();

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
