namespace Masa.Contrib.Isolation.UoW.EF;

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
    private readonly ILogger<IsolationDbContext<TKey>>? _logger;

    public IsolationDbContext(MasaDbContextOptions options) : base(options)
    {
        _environmentContext = options.ServiceProvider.GetService<IEnvironmentContext>();
        _tenantContext = options.ServiceProvider.GetService<ITenantContext>();
        _logger = options.ServiceProvider.GetService<ILogger<IsolationDbContext<TKey>>>();
    }

    protected override Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? expression = null;

        if (typeof(IMultiTenant<>).IsGenericInterfaceAssignableFrom(typeof(TEntity)) && _tenantContext != null)
        {
            Expression<Func<TEntity, bool>> tenantFilter = entity => !IsTenantFilterEnabled ||
                _tenantContext.CurrentTenant == null ||
                _tenantContext.CurrentTenant.Id == string.Empty ||
                _tenantContext.CurrentTenant.Id == null! ||
                Microsoft.EntityFrameworkCore.EF.Property<TKey>(entity, nameof(IMultiTenant<TKey>.TenantId)).ToString() ==
                (_tenantContext.CurrentTenant != null ? _tenantContext.CurrentTenant.Id : default(TKey)!.ToString());
            expression = tenantFilter.And(expression != null, expression);
        }

        if (typeof(IEnvironment).IsAssignableFrom(typeof(TEntity)) && _environmentContext != null)
        {
            Expression<Func<TEntity, bool>> envFilter = entity => !IsEnvironmentFilterEnabled ||
                _environmentContext.CurrentEnvironment == string.Empty ||
                _environmentContext.CurrentEnvironment == null! ||
                Microsoft.EntityFrameworkCore.EF.Property<string>(entity, nameof(IEnvironment.Environment))
                    .Equals(_environmentContext == null ? default : _environmentContext.CurrentEnvironment);
            expression = envFilter.And(expression != null, expression);
        }

        var secondExpression = base.CreateFilterExpression<TEntity>();
        if (secondExpression != null)
            expression = secondExpression.And(expression != null, expression);

        return expression;
    }

    protected virtual bool IsEnvironmentFilterEnabled => DataFilter?.IsEnabled<IEnvironment>() ?? false;

    protected virtual bool IsTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant<TKey>>() ?? false;
}
