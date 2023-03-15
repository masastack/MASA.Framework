// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public abstract class MasaDbContext<TDbContext> : DbContext, IMasaDbContext
    where TDbContext : IMasaDbContext
{
    private bool _initialized;
    private IDataFilter? _dataFilter;

    protected IDataFilter? DataFilter
    {
        get
        {
            TryInitialize();
            return _dataFilter;
        }
    }

    protected readonly MasaDbContextOptions Options;

    private IDomainEventBus? _domainEventBus;

    protected IDomainEventBus? DomainEventBus
    {
        get
        {
            TryInitialize();
            return _domainEventBus;
        }
    }

    private IConcurrencyStampProvider? _concurrencyStampProvider;

    public IConcurrencyStampProvider? ConcurrencyStampProvider
    {
        get
        {
            TryInitialize();
            return _concurrencyStampProvider;
        }
    }

    protected MasaDbContext(MasaDbContextOptions<TDbContext> options) : base(options)
    {
    }

    protected virtual void TryInitialize()
    {
        if (!_initialized) Initialize();
    }

    protected virtual void Initialize()
    {
        _dataFilter = Options.ServiceProvider?.GetService<IDataFilter>();
        _domainEventBus = Options.ServiceProvider?.GetService<IDomainEventBus>();
        _concurrencyStampProvider = Options.ServiceProvider?.GetRequiredService<IConcurrencyStampProvider>();
        _initialized = true;
    }

    /// <summary>
    /// Automatic filter soft delete data.
    /// When you override this method,you should call base.<see cref="OnModelCreating(ModelBuilder)"/>.
    /// <inheritdoc/>
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingExecuting(modelBuilder);

        OnModelCreatingConfigureGlobalFilters(modelBuilder);

        foreach (var provider in Options.ModelCreatingProviders)
            provider.Configure(modelBuilder);
    }

    /// <summary>
    /// Use this method instead of OnModelCreating
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected virtual void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {

    }

    protected virtual void OnModelCreatingConfigureGlobalFilters(ModelBuilder modelBuilder)
    {
        var methodInfo = GetType().GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            methodInfo!.MakeGenericMethod(entityType.ClrType).Invoke(this, new object?[]
            {
                modelBuilder, entityType
            });
        }
    }

    protected virtual void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class
    {
        if (mutableEntityType.BaseType == null)
        {
            var filterExpression = CreateFilterExpression<TEntity>();
            if (filterExpression != null)
                modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
        }
    }

    protected virtual Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? expression = null;

        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            expression = entity => !IsSoftDeleteFilterEnabled || !EF.Property<bool>(entity, nameof(ISoftDelete.IsDeleted));
        }

        return expression;
    }

    protected virtual bool IsSoftDeleteFilterEnabled
        => Options is { EnableSoftDelete: true } && (DataFilter?.IsEnabled<ISoftDelete>() ?? false);

    /// <summary>
    /// Automatic soft delete.
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int SaveChanges() => SaveChanges(true);

    public sealed override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaveChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    protected virtual void OnBeforeSaveChanges()
    {
        UpdateRowVersion(ChangeTracker);
        OnBeforeSaveChangesByFilters();
        DomainEventEnqueueAsync(ChangeTracker).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    protected virtual async Task OnBeforeSaveChangesAsync()
    {
        UpdateRowVersion(ChangeTracker);
        OnBeforeSaveChangesByFilters();
        await DomainEventEnqueueAsync(ChangeTracker);
    }

    protected virtual void OnBeforeSaveChangesByFilters()
    {
        foreach (var filter in Options.SaveChangesFilters)
        {
            try
            {
                filter.OnExecuting(ChangeTracker);
            }
            catch (Exception ex)
            {
                throw new MasaException("An error occured when intercept SaveChanges() or SaveChangesAsync()", ex);
            }
        }
    }

    protected virtual async Task DomainEventEnqueueAsync(ChangeTracker changeTracker)
    {
        if (DomainEventBus == null)
            return;

        var domainEntities = changeTracker
            .Entries<IGenerateDomainEvents>()
            .Where(entry => entry.Entity.GetDomainEvents().Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(entry => entry.Entity.GetDomainEvents())
            .ToList();

        domainEntities
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await DomainEventBus.Enqueue(domainEvent);
    }

    protected virtual void UpdateRowVersion(ChangeTracker changeTracker)
    {
        if (ConcurrencyStampProvider == null)
            return;

        var entries = changeTracker.Entries().Where(entry
            => (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted) &&
            entry.Entity is IHasConcurrencyStamp);
        foreach (var entity in entries)
        {
            entity.CurrentValues[nameof(IHasConcurrencyStamp.RowVersion)] = ConcurrencyStampProvider.GetRowVersion();
        }
    }

    /// <summary>
    /// Automatic soft delete.
    /// <inheritdoc/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => SaveChangesAsync(true, cancellationToken);

    /// <summary>
    /// Automatic soft delete.
    /// <inheritdoc/>
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public sealed override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        await OnBeforeSaveChangesAsync();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
