// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EFCore;

public class IsolationSaveChangesFilter<TTenantId> : ISaveChangesFilter where TTenantId : IComparable
{
    private readonly ITenantContext? _tenantContext;
    private readonly IConvertProvider? _convertProvider;
    private readonly IEnvironmentContext? _environmentContext;

    public IsolationSaveChangesFilter(IServiceProvider serviceProvider)
    {
        _tenantContext = serviceProvider.GetService<ITenantContext>();
        _convertProvider = serviceProvider.GetService<IConvertProvider>();
        _environmentContext = serviceProvider.GetService<IEnvironmentContext>();
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();
        var entries = changeTracker.Entries().Where(entry => entry.State == EntityState.Added);
        foreach (var entity in entries)
        {
            if (entity.Entity is IMultiTenant<TTenantId> && _tenantContext != null)
            {
                if (_tenantContext.CurrentTenant != null && !string.IsNullOrEmpty(_tenantContext.CurrentTenant.Id))
                {
                    ArgumentNullException.ThrowIfNull(_convertProvider, nameof(_convertProvider));
                    object tenantId = _convertProvider.ChangeType(_tenantContext.CurrentTenant.Id, typeof(TTenantId));
                    entity.CurrentValues[nameof(IMultiTenant<TTenantId>.TenantId)] = tenantId;
                }
                else
                {
                    entity.CurrentValues[nameof(IMultiTenant<TTenantId>.TenantId)] = default(TTenantId);
                }
            }

            if (entity.Entity is IMultiEnvironment && _environmentContext != null)
            {
                entity.CurrentValues[nameof(IMultiEnvironment.Environment)] = _environmentContext.CurrentEnvironment;
            }
        }
    }
}
