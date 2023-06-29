// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

[ExcludeFromCodeCoverage]
internal class IsolationSaveChangesFilter<TDbContext, TTenantId> : ISaveChangesFilter<TDbContext>
    where TDbContext : DbContext, IMasaDbContext
    where TTenantId : IComparable
{
    private readonly IMultiTenantContext? _tenantContext;
    private readonly ITypeConvertProvider _convertProvider;
    private readonly IMultiEnvironmentContext? _environmentContext;

    public IsolationSaveChangesFilter(IServiceProvider serviceProvider)
    {
        _tenantContext = serviceProvider.GetService<IMultiTenantContext>();
        _convertProvider = serviceProvider.GetService<ITypeConvertProvider>() ?? new DefaultTypeConvertProvider();
        _environmentContext = serviceProvider.GetService<IMultiEnvironmentContext>();
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();
        var entries = changeTracker.Entries().Where(entry => entry.State == Microsoft.EntityFrameworkCore.EntityState.Added);

        bool initialized = false;
        object? tenantId = null;
        foreach (var entity in entries)
        {
            if (entity.Entity.GetType().IsImplementerOfGeneric(typeof(IMultiTenant<>)) &&
                ((initialized && tenantId != null) || !initialized))
            {
                if (!initialized)
                {
                    tenantId = GetTenantId();
                    initialized = true;
                }
                if (tenantId != null)
                {
                    entity.CurrentValues[nameof(IMultiTenant<TTenantId>.TenantId)] = tenantId;
                }
            }

            if (entity.Entity is IMultiEnvironment && _environmentContext != null)
            {
                entity.CurrentValues[nameof(IMultiEnvironment.Environment)] = _environmentContext.CurrentEnvironment;
            }
        }
    }

    private object? GetTenantId()
    {
        if (_tenantContext is { CurrentTenant: not null } && !string.IsNullOrWhiteSpace(_tenantContext.CurrentTenant.Id))
        {
            ArgumentNullException.ThrowIfNull(_convertProvider, nameof(_convertProvider));
            return _convertProvider.ConvertTo(_tenantContext.CurrentTenant.Id, typeof(TTenantId));
        }
        return null;
    }
}
