// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class SaveChangeFilter<TDbContext, TUserId> : ISaveChangesFilter<TDbContext>
    where TDbContext : DbContext, IMasaDbContext
{
    private readonly Type _userIdType;
    private readonly IUserContext? _userContext;
    private readonly ITypeAndDefaultValueProvider _typeAndDefaultValueProvider;
    private readonly ITypeConvertProvider _typeConvertProvider;

    public SaveChangeFilter(
        IUserContext? userContext = null,
        ITypeAndDefaultValueProvider? typeAndDefaultValueProvider = null,
        ITypeConvertProvider? typeConvertProvider = null)
    {
        _userIdType = typeof(TUserId);
        _userContext = userContext;
        _typeAndDefaultValueProvider = typeAndDefaultValueProvider ?? new DefaultTypeAndDefaultValueProvider();
        _typeConvertProvider = typeConvertProvider ?? new DefaultTypeConvertProvider();

        _typeAndDefaultValueProvider.TryAdd(_userIdType);
        _typeAndDefaultValueProvider.TryAdd(typeof(DateTime));
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();

        var userId = GetUserId(_userContext?.UserId);

        _typeAndDefaultValueProvider.TryGet(_userIdType, out string? defaultUserId);
        _typeAndDefaultValueProvider.TryGet(typeof(DateTime), out string? defaultDateTime);

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
            entity.CurrentValues[nameof(IAuditEntity<TUserId>.ModificationTime)] =
                DateTime.UtcNow; //The current time to change to localization after waiting for localization
        }
    }

    private static bool IsDefault(object? value, string? defaultValue)
        => value == null || value.ToString() == defaultValue;

    /// <summary>
    /// Get the current user id
    /// Does not consider user id as DateTime type
    /// </summary>
    /// <returns></returns>
    private object? GetUserId(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        return _typeConvertProvider.ConvertTo(userId, _userIdType);
    }
}
