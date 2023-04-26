// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class SaveChangeFilter<TDbContext, TUserId> : ISaveChangesFilter<TDbContext>
    where TDbContext : DbContext, IMasaDbContext
{
    private readonly Type _userIdType;
    private readonly IUserContext? _userContext;

    public SaveChangeFilter(IUserContext? userContext = null)
    {
        _userIdType = typeof(TUserId);
        _userContext = userContext;
        Masa.Contrib.Data.EFCore.TypeExtensions.TypeAndDefaultValues.TryAdd(_userIdType, type => Activator.CreateInstance(type)?.ToString());
        Masa.Contrib.Data.EFCore.TypeExtensions.TypeAndDefaultValues.TryAdd(typeof(DateTime), type => Activator.CreateInstance(type)?.ToString());
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();

        var userId = GetUserId(_userContext?.UserId);

        var defaultUserId = Masa.Contrib.Data.EFCore.TypeExtensions.TypeAndDefaultValues[_userIdType];
        var defaultDateTime = Masa.Contrib.Data.EFCore.TypeExtensions.TypeAndDefaultValues[typeof(DateTime)];

        foreach (var entity in changeTracker.Entries()
                     .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified))
        {
            AuditEntityHandler(entity, userId, defaultUserId, defaultDateTime);
        }
    }

    private static void AuditEntityHandler(EntityEntry entity, object? userId, string? defaultUserId, string? defaultDateTime)
    {
        if (entity.Entity.GetType().IsImplementerOfGeneric(typeof(IAuditEntity<>)))
        {
            if (entity.State == EntityState.Added)
            {
                AuditEntityHandlerByAdded(entity, userId, defaultUserId, defaultDateTime);
            }
            else
            {
                if (userId != null)
                    entity.CurrentValues[nameof(IAuditEntity<TUserId>.Modifier)] = userId;

                entity.CurrentValues[nameof(IAuditEntity<TUserId>.ModificationTime)] =
                    DateTime.UtcNow; //The current time to change to localization after waiting for localization
            }
        }
    }

    private static void AuditEntityHandlerByAdded(EntityEntry entity, object? userId, string? defaultUserId, string? defaultDateTime)
    {
        if (userId != null)
        {
            if (IsDefault(entity.CurrentValues[nameof(IAuditEntity<TUserId>.Creator)], defaultUserId))
            {
                entity.CurrentValues[nameof(IAuditEntity<TUserId>.Creator)] = userId;
            }

            if (IsDefault(entity.CurrentValues[nameof(IAuditEntity<TUserId>.Modifier)], defaultUserId))
            {
                entity.CurrentValues[nameof(IAuditEntity<TUserId>.Modifier)] = userId;
            }
        }

        if (IsDefault(entity.CurrentValues[nameof(IAuditEntity<TUserId>.CreationTime)], defaultDateTime))
        {
            entity.CurrentValues[nameof(IAuditEntity<TUserId>.CreationTime)] =
                DateTime.UtcNow; //The current time to change to localization after waiting for localization
        }

        if (IsDefault(entity.CurrentValues[nameof(IAuditEntity<TUserId>.ModificationTime)], defaultDateTime))
        {
            entity.CurrentValues[nameof(IAuditEntity<TUserId>.ModificationTime)] ??=
                DateTime.UtcNow; //The current time to change to localization after waiting for localization
        }
    }

    private static bool IsDefault(object? value, string? defaultValue)
        => value == null || value.ToString() == defaultValue;

    private object? GetUserId(string? userId)
    {
        if (userId == null)
            return null;

        if (_userIdType == typeof(Guid))
            return Guid.Parse(userId);

        return Convert.ChangeType(userId, _userIdType);
    }
}
