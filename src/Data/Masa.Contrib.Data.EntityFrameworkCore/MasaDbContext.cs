// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public abstract class MasaDbContext : DbContext, IMasaDbContext
{
    protected readonly IDataFilter? DataFilter;
    protected readonly MasaDbContextOptions Options;
    protected IDomainEventBus? DomainEventBus => Options.ServiceProvider.GetService<IDomainEventBus>();

    public MasaDbContext(MasaDbContextOptions options) : base(options)
    {
        Options = options;
        DataFilter = options.ServiceProvider.GetService<IDataFilter>();
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

        // null when run dotnet ef cli
        if (Options == null)
        {
            base.OnModelCreating(modelBuilder);
            return;
        }

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
        var methodInfo = typeof(MasaDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            methodInfo!.MakeGenericMethod(entityType.ClrType).Invoke(this, new object?[] { modelBuilder, entityType });
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
        => (Options?.EnableSoftDelete ?? false) && (DataFilter?.IsEnabled<ISoftDelete>() ?? false);

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
        if (Options != null)
        {
            OnBeforeSaveChangesByFilters();
            DomainEventEnqueueAsync(ChangeTracker).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    protected virtual async Task OnBeforeSaveChangesAsync()
    {
        if (Options != null)
        {
            OnBeforeSaveChangesByFilters();
            await DomainEventEnqueueAsync(ChangeTracker);
        }
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
                throw new Exception("An error occured when intercept SaveChanges() or SaveChangesAsync()", ex);
            }
        }
    }

    protected virtual async Task DomainEventEnqueueAsync(ChangeTracker changeTracker)
    {
        if (DomainEventBus == null)
            return;

        var domainEntities = changeTracker
            .Entries<IGenerateDomainEvents>()
            .Where(entry => entry.Entity.GetDomainEvents().Any());

        var domainEvents = domainEntities
            .SelectMany(entry => entry.Entity.GetDomainEvents())
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await DomainEventBus.Enqueue(domainEvent);
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
    public sealed override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}

public abstract class MasaDbContext<TDbContext> : MasaDbContext
    where TDbContext : DbContext, IMasaDbContext
{
    public MasaDbContext(MasaDbContextOptions<TDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreatingConfigureGlobalFilters(ModelBuilder modelBuilder)
    {
        var methodInfo = typeof(MasaDbContext<TDbContext>).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            methodInfo!.MakeGenericMethod(entityType.ClrType).Invoke(this, new object?[] { modelBuilder, entityType });
        }
    }
}
