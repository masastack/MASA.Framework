// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EF;

/// <summary>
/// DbContext providing isolation
/// </summary>
/// <typeparam name="TKey">tenant id type</typeparam>
/// <typeparam name="TDbContext"></typeparam>
public abstract class IsolationDbContext<TDbContext, TKey> : IsolationDbContext<TKey>
    where TKey : IComparable
    where TDbContext : DbContext, IMasaDbContext
{
    protected IsolationDbContext(MasaDbContextOptions<TDbContext> options) : base(options)
    {
    }
}

/// <summary>
/// DbContext providing isolation
/// </summary>
/// <typeparam name="TKey">tenant id type</typeparam>
public abstract class IsolationDbContext : IsolationDbContext<Guid>
{
    protected IsolationDbContext(MasaDbContextOptions options) : base(options)
    {
    }
}

/// <summary>
/// DbContext providing isolation
/// </summary>
/// <typeparam name="TKey">tenant id type</typeparam>
public abstract class IsolationDbContext<TKey> : MasaDbContext
    where TKey : IComparable
{
    private readonly IEnvironmentContext? _environmentContext;
    private readonly ITenantContext? _tenantContext;

    public IsolationDbContext(MasaDbContextOptions options) : base(options)
    {
        _environmentContext = options.ServiceProvider.GetService<IEnvironmentContext>();
        _tenantContext = options.ServiceProvider.GetService<ITenantContext>();
    }

    protected override Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? expression = null;

        if (typeof(IMultiTenant<>).IsGenericInterfaceAssignableFrom(typeof(TEntity)) && _tenantContext != null)
        {
            string defaultTenantId = default(TKey)?.ToString() ?? string.Empty;
            Expression<Func<TEntity, bool>> tenantFilter = entity => !IsTenantFilterEnabled ||
                (Microsoft.EntityFrameworkCore.EF.Property<TKey>(entity, nameof(IMultiTenant<TKey>.TenantId)).ToString() ?? string.Empty)
                .Equals(_tenantContext.CurrentTenant != null ? _tenantContext.CurrentTenant.Id : defaultTenantId);
            expression = tenantFilter.And(expression != null, expression);
        }

        if (typeof(IMultiEnvironment).IsAssignableFrom(typeof(TEntity)) && _environmentContext != null)
        {
            Expression<Func<TEntity, bool>> envFilter = entity => !IsEnvironmentFilterEnabled ||
                Microsoft.EntityFrameworkCore.EF.Property<string>(entity, nameof(IMultiEnvironment.Environment))
                    .Equals(_environmentContext != null ? _environmentContext.CurrentEnvironment : default);
            expression = envFilter.And(expression != null, expression);
        }

        var secondExpression = base.CreateFilterExpression<TEntity>();
        if (secondExpression != null)
            expression = secondExpression.And(expression != null, expression);

        return expression;
    }

    protected virtual bool IsEnvironmentFilterEnabled => DataFilter?.IsEnabled<IMultiEnvironment>() ?? false;

    protected virtual bool IsTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant<TKey>>() ?? false;
}
