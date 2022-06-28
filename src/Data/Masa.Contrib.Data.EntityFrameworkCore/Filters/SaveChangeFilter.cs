// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Filters;

public class SaveChangeFilter<TDbContext, TUserId> : ISaveChangesFilter
    where TDbContext : DbContext
{
    private readonly Type _userIdType;
    private readonly IUserContext? _userContext;

    public SaveChangeFilter(IUserContext? userContext = null)
    {
        _userIdType = typeof(TUserId);
        _userContext = userContext;
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();

        foreach (var entity in changeTracker.Entries()
                     .Where(entry => entry.Entity is IAuditEntity<TUserId> &&
                         (entry.State == EntityState.Added || entry.State == EntityState.Modified)))
        {
            var userId = GetUserId(_userContext?.UserId);
            if (entity.State == EntityState.Added)
            {
                if (userId != null)
                {
                    entity.CurrentValues[nameof(IAuditEntity<TUserId>.Creator)] = userId;
                    entity.CurrentValues[nameof(IAuditEntity<TUserId>.Modifier)] = userId;
                }
                entity.CurrentValues[nameof(IAuditEntity<TUserId>.CreationTime)] =
                    DateTime.UtcNow; //The current time to change to localization after waiting for localization
            }
            else if (entity.CurrentValues[nameof(IAuditEntity<TUserId>.Modifier)] != default && userId != null)
            {
                entity.CurrentValues[nameof(IAuditEntity<TUserId>.Modifier)] = userId;
            }
            entity.CurrentValues[nameof(IAuditEntity<TUserId>.ModificationTime)] =
                DateTime.UtcNow; //The current time to change to localization after waiting for localization
        }
    }

    private object? GetUserId(string? userId)
    {
        if (userId == null)
            return null;

        if (_userIdType == typeof(Guid))
            return Guid.Parse(userId);

        return Convert.ChangeType(userId, _userIdType);
    }
}
