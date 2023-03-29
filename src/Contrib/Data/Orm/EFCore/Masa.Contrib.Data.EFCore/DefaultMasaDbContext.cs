// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// This is an internal API backing the MasaDbContext infrastructure and is not subject to the same compatibility standards as the public API. It may be changed or removed without notice
/// </summary>
public class DefaultMasaDbContext : DbContext, IMasaDbContext
{
    private bool _initialized;
    private IDataFilter? _dataFilter;

    protected IDataFilter? DataFilter => _dataFilter;

    protected MasaDbContextOptions? Options { get; private set; }

    private IDomainEventBus? _domainEventBus;

    protected IDomainEventBus? DomainEventBus => _domainEventBus;

    private IConcurrencyStampProvider? _concurrencyStampProvider;

    public IConcurrencyStampProvider? ConcurrencyStampProvider => _concurrencyStampProvider;

    private IMultiEnvironmentContext? EnvironmentContext => Options?.ServiceProvider?.GetService<IMultiEnvironmentContext>();

    protected IMultiTenantContext? TenantContext => Options?.ServiceProvider?.GetService<IMultiTenantContext>();

    protected virtual bool IsEnvironmentFilterEnabled => DataFilter?.IsEnabled<IMultiEnvironment>() ?? false;

    protected virtual bool IsTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant<Guid>>() ?? false;

    protected DefaultMasaDbContext()
    {
    }

    public DefaultMasaDbContext(MasaDbContextOptions options) : base(options)
    {
        Options = options;
    }

    internal void TryInitialize(MasaDbContextOptions? options)
    {
        if (_initialized)
            return;

        if (options != null)
            Options ??= options;

        Initialize();
    }

    protected virtual void Initialize()
    {
        _dataFilter = Options!.ServiceProvider?.GetService<IDataFilter>();
        _domainEventBus = Options!.ServiceProvider?.GetService<IDomainEventBus>();
        _concurrencyStampProvider = Options!.ServiceProvider?.GetRequiredService<IConcurrencyStampProvider>();
        _initialized = true;

        base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
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

        foreach (var provider in Options!.ModelCreatingProviders)
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

        if (typeof(IMultiEnvironment).IsAssignableFrom(typeof(TEntity)) && EnvironmentContext != null)
        {
            Expression<Func<TEntity, bool>> envFilter = entity => !IsEnvironmentFilterEnabled ||
                EF.Property<string>(entity, nameof(IMultiEnvironment.Environment))
                    .Equals(EnvironmentContext != null ? EnvironmentContext.CurrentEnvironment : default);
            expression = envFilter.And(expression != null, expression);
        }

        expression = CreateMultiTenantFilterExpression(expression);

        return expression;
    }

    protected virtual Expression<Func<TEntity, bool>>? CreateMultiTenantFilterExpression<TEntity>(
        Expression<Func<TEntity, bool>>? expression)
        where TEntity : class
    {
        if (typeof(IMultiTenant<>).IsGenericInterfaceAssignableFrom(typeof(TEntity)) && TenantContext != null)
        {
            string defaultTenantId = Guid.Empty.ToString();
            Expression<Func<TEntity, bool>> tenantFilter = entity => !IsTenantFilterEnabled ||
                (EF.Property<Guid>(entity, nameof(IMultiTenant<Guid>.TenantId)).ToString())
                .Equals(TenantContext.CurrentTenant != null ? TenantContext.CurrentTenant.Id : defaultTenantId);

            expression = tenantFilter.And(expression != null, expression);
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
        ChangeTracker.UpdateRowVersion(ConcurrencyStampProvider);
        OnBeforeSaveChangesByFilters();
        DomainEventEnqueueAsync(ChangeTracker).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    protected virtual async Task OnBeforeSaveChangesAsync()
    {
        ChangeTracker.UpdateRowVersion(ConcurrencyStampProvider);
        OnBeforeSaveChangesByFilters();
        await DomainEventEnqueueAsync(ChangeTracker);
    }

    protected virtual void OnBeforeSaveChangesByFilters()
    {
        foreach (var filter in Options!.SaveChangesFilters)
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
            await DomainEventBus.EnqueueAsync(domainEvent);
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

    protected sealed override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (Options == null)
            return;

        var masaDbContextOptionsBuilder = new MasaDbContextOptionsBuilder(Options?.ServiceProvider, optionsBuilder, Options?.DbContextType);
        OnConfiguring(masaDbContextOptionsBuilder);
    }

    protected virtual void OnConfiguring(MasaDbContextOptionsBuilder optionsBuilder)
    {
    }
}

/// <summary>
/// This is an internal API backing the MasaDbContext infrastructure and is not subject to the same compatibility standards as the public API. It may be changed or removed without notice
/// </summary>
/// <typeparam name="TMultiTenantId"></typeparam>
public class DefaultMasaDbContext<TMultiTenantId> : DefaultMasaDbContext
    where TMultiTenantId : IComparable
{
    protected override bool IsTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant<TMultiTenantId>>() ?? false;

    protected DefaultMasaDbContext()
    {
    }

    public DefaultMasaDbContext(MasaDbContextOptions options) : base(options)
    {
    }

    protected sealed override Expression<Func<TEntity, bool>>? CreateMultiTenantFilterExpression<TEntity>(
        Expression<Func<TEntity, bool>>? expression)
        where TEntity : class
    {
        if (typeof(IMultiTenant<>).IsGenericInterfaceAssignableFrom(typeof(TEntity)) && TenantContext != null)
        {
            string defaultTenantId = default(TMultiTenantId)?.ToString() ?? string.Empty;
            Expression<Func<TEntity, bool>> tenantFilter = entity => !IsTenantFilterEnabled ||
                (EF.Property<TMultiTenantId>(entity, nameof(IMultiTenant<TMultiTenantId>.TenantId)).ToString() ?? string.Empty)
                .Equals(TenantContext.CurrentTenant != null ? TenantContext.CurrentTenant.Id : defaultTenantId);

            expression = tenantFilter.And(expression != null, expression);
        }
        return expression;
    }
}
