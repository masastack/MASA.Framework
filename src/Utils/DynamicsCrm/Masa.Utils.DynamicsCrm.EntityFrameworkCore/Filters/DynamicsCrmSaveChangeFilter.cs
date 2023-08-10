// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.DynamicsCrm.EntityFrameworkCore.Filters;

public class DynamicsCrmSaveChangeFilter<TDbContext, TUserId> : ISaveChangesFilter<TDbContext>
    where TDbContext : DbContext, IMasaDbContext
{
    private readonly Type _userIdType;
    private readonly IUserContext? _userContext;
    private readonly ITypeAndDefaultValueProvider _typeAndDefaultValueProvider;
    private readonly ITypeConvertProvider _typeConvertProvider;
    private readonly ICrmConfiguration? _crmConfiguration;

    public DynamicsCrmSaveChangeFilter(
        IUserContext? userContext = null,
        ITypeAndDefaultValueProvider? typeAndDefaultValueProvider = null,
        ITypeConvertProvider? typeConvertProvider = null,
        ICrmConfiguration? crmConfiguration = null)
    {
        _userIdType = typeof(TUserId);
        _userContext = userContext;
        _typeAndDefaultValueProvider = typeAndDefaultValueProvider ?? new DefaultTypeAndDefaultValueProvider();
        _typeConvertProvider = typeConvertProvider ?? new DefaultTypeConvertProvider();

        _typeAndDefaultValueProvider.TryAdd(_userIdType);
        _typeAndDefaultValueProvider.TryAdd(typeof(DateTime));
        _crmConfiguration = crmConfiguration;
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();

        var user = _userContext?.GetUser<DynamicsCrmUser>();
        Guid? userId = user != null ? _typeConvertProvider.ConvertTo<Guid>(user.Id) : null;
        var systemUserId = userId ?? _crmConfiguration?.SystemUserId ?? default;
        var businessUnitId = user?.BusinessUnitId ?? _crmConfiguration?.BusinessUnitId ?? default;

        foreach (var entity in changeTracker.Entries()
                     .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified))
        {
            AuditEntityHandler(entity, systemUserId, businessUnitId);
        }
    }

    private static void AuditEntityHandler(EntityEntry entity, Guid systemUserId, Guid businessUnitId)
    {
        if (entity.State == EntityState.Added)
        {
            CrmEntityAuditingHelper.SetCreationAuditProperties(entity.Entity, systemUserId, systemUserId, businessUnitId);
        }
        else
        {
            CrmEntityAuditingHelper.SetModificationAuditProperties(entity.Entity, systemUserId);
        }
    }
}