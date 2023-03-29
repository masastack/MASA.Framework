// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class IsolationDbContext : MasaDbContext
{
    private IMultiEnvironmentContext? _environmentContext;
    private IMultiTenantContext? _tenantContext;

    protected virtual bool IsEnvironmentFilterEnabled => DataFilter?.IsEnabled<IMultiEnvironment>() ?? false;

    protected virtual bool IsTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant<Guid>>() ?? false;

    protected IsolationDbContext() : base()
    {
    }

    protected IsolationDbContext(MasaDbContextOptions options) : base(options)
    {
    }

    protected override void Initialize()
    {
        _environmentContext = Options?.ServiceProvider?.GetService<IMultiEnvironmentContext>();
        _tenantContext = Options?.ServiceProvider?.GetService<IMultiTenantContext>();
        base.Initialize();
    }

    protected override Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? expression = null;

        if (typeof(IMultiTenant<>).IsGenericInterfaceAssignableFrom(typeof(TEntity)) && _tenantContext != null)
        {
            string defaultTenantId = string.Empty;//todo: It is necessary to determine whether the tenant id supports nullability
            Expression<Func<TEntity, bool>> tenantFilter = entity => !IsTenantFilterEnabled ||
                (EF.Property<Guid>(entity, nameof(IMultiTenant<Guid>.TenantId)).ToString() ?? string.Empty)
                .Equals(_tenantContext.CurrentTenant != null ? _tenantContext.CurrentTenant.Id : defaultTenantId);

            expression = tenantFilter.And(expression != null, expression);
        }

        if (typeof(IMultiEnvironment).IsAssignableFrom(typeof(TEntity)) && _environmentContext != null)
        {
            Expression<Func<TEntity, bool>> envFilter = entity => !IsEnvironmentFilterEnabled ||
                EF.Property<string>(entity, nameof(IMultiEnvironment.Environment))
                    .Equals(_environmentContext != null ? _environmentContext.CurrentEnvironment : default);
            expression = envFilter.And(expression != null, expression);
        }

        var secondExpression = base.CreateFilterExpression<TEntity>();
        if (secondExpression != null)
            expression = secondExpression.And(expression != null, expression);

        return expression;
    }
}

public class IsolationDbContext<TDbContext> : IsolationDbContext<TDbContext, Guid>
    where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
{
    protected IsolationDbContext() : base()
    {
    }

    public IsolationDbContext(MasaDbContextOptions<TDbContext> options) : base(options)
    {
    }
}

public class IsolationDbContext<TDbContext, TMultiTenantId> : MasaDbContext<TDbContext>
    where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
    where TMultiTenantId : IComparable
{
    protected IsolationDbContext() : base()
    {
    }

    public IsolationDbContext(MasaDbContextOptions<TDbContext> options) : base(options)
    {
    }
}
