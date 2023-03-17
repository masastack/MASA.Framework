// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore;

internal class MultiTenantProvider
{
    public Type? MultiTenantIdType { get; }

    public string MultiTenantIdDefaultValue { get; }

    private readonly MemoryCache<Type, MethodInfo> _data = new();
    private readonly MethodInfo? _isEnabledMethodInfo;

    public MultiTenantProvider(IServiceProvider serviceProvider)
    {
        MultiTenantIdType = serviceProvider.GetService<IOptions<IsolationOptions>>()?.Value.MultiTenantType;
        MultiTenantIdDefaultValue = MultiTenantIdType?.GetDefaultValue()?.ToString() ?? string.Empty;
        if (MultiTenantIdType != null)
            _isEnabledMethodInfo = typeof(IDataFilter)
                .GetMethod(nameof(IDataFilter.IsEnabled))!
                .MakeGenericMethod(typeof(IMultiTenant<>).MakeGenericType(MultiTenantIdType));
    }

    public string GetMultiTenantId<TEntity>(TEntity entity)
    {
        var genericMethod = _data.GetOrAdd(typeof(TEntity),
            entityType => entityType.GetMethod(nameof(EF.Property))!.MakeGenericMethod(MultiTenantIdType!));
        return genericMethod.Invoke(entity, new object[]
        {
            nameof(IMultiTenant<Guid>.TenantId)
        })?.ToString() ?? string.Empty;
    }

    public bool IsTenantFilterEnabled(IDataFilter dataFilter)
    {
        var result = _isEnabledMethodInfo!.Invoke(dataFilter, Array.Empty<object>());
        if (result == null)
            return false;

        return (bool)result;
    }
}
