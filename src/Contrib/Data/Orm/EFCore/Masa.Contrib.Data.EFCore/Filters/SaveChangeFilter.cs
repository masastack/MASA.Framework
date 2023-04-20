﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class SaveChangeFilter<TDbContext, TUserId> : ISaveChangesFilter<TDbContext>
    where TDbContext : DbContext, IMasaDbContext
{
    private readonly Type _userIdType;
    private readonly IUserContext? _userContext;
    private static readonly MemoryCache<Type, object?> _typeAndDefaultValues = new();

    public SaveChangeFilter(IUserContext? userContext = null)
    {
        _userIdType = typeof(TUserId);
        _userContext = userContext;
        _typeAndDefaultValues.TryAdd(_userIdType, Activator.CreateInstance);
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();

        var userId = GetUserId(_userContext?.UserId);

        var defaultUserId = _typeAndDefaultValues[_userIdType];
        foreach (var entity in changeTracker.Entries()
                     .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified))
        {
            AuditEntityHandler(entity, userId, defaultUserId);
        }
    }

    private static void AuditEntityHandler(EntityEntry entity, object? userId, object? defaultUserId)
    {
        if (entity.Entity.GetType().IsImplementerOfGeneric(typeof(IAuditEntity<>)))
        {
            if (entity.State == EntityState.Added)
            {
                if (userId != null)
                {
                    if (entity.CurrentValues[nameof(IAuditEntity<TUserId>.Creator)] != defaultUserId)
                    {
                        entity.CurrentValues[nameof(IAuditEntity<TUserId>.Creator)] = userId;
                    }
                    if (entity.CurrentValues[nameof(IAuditEntity<TUserId>.Modifier)] != defaultUserId)
                    {
                        entity.CurrentValues[nameof(IAuditEntity<TUserId>.Modifier)] = userId;
                    }
                }

                entity.CurrentValues[nameof(IAuditEntity<TUserId>.CreationTime)] ??=
                    DateTime.UtcNow; //The current time to change to localization after waiting for localization
                entity.CurrentValues[nameof(IAuditEntity<TUserId>.ModificationTime)] ??=
                    DateTime.UtcNow; //The current time to change to localization after waiting for localization
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

    private object? GetUserId(string? userId)
    {
        if (userId == null)
            return null;

        if (_userIdType == typeof(Guid))
            return Guid.Parse(userId);

        return Convert.ChangeType(userId, _userIdType);
    }
}
