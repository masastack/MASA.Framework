// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.DynamicsCrm.EntityFrameworkCore.Filters;

public class DynamicsCrmSaveChangeFilter<TDbContext, TUserId> : ISaveChangesFilter<TDbContext>
    where TDbContext : DbContext, IMasaDbContext
{
    private readonly IUserContext? _userContext;
    private readonly ICrmConfiguration? _crmConfiguration;

    public DynamicsCrmSaveChangeFilter(
        IUserContext? userContext = null,
        ICrmConfiguration? crmConfiguration = null)
    {
        _userContext = userContext;
        _crmConfiguration = crmConfiguration;
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();

        var user = _userContext?.GetUser<DynamicsCrmUser>();
        var userId = GetUserId();
        var businessUnitId = user?.BusinessUnitId ?? _crmConfiguration?.BusinessUnitId ?? Guid.Empty;

        foreach (var entity in changeTracker.Entries()
                     .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified))
        {
            AuditEntityHandler(entity, userId, businessUnitId);
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

    private Guid GetUserId()
    {
        var defaultUserId = _crmConfiguration?.SystemUserId ?? Guid.Empty;

        if (_userContext == null)
            return defaultUserId;

        var tUserId = _userContext.GetUserId<TUserId>();

        if (tUserId == null || tUserId is Guid == false)
            return defaultUserId;

        var userId = tUserId as Guid?;
        if (userId == null || userId == Guid.Empty)
            return defaultUserId;

        return userId.Value;
    }
}
