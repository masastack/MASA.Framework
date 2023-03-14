// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.EFCore")]
// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

internal static class ModuleConfigUtils
{
    private static readonly MemoryCache<Type, PropertyInfo?> _data = new();

    public static bool TryGetConfig<TConfig>(
        object module,
        string propertyName,
        [NotNullWhen(true)] out TConfig? config) where TConfig : class
    {
        MasaArgumentException.ThrowIfNull(module);

        var module1 = module;
        var propertyInfo = _data.GetOrAdd(typeof(TConfig), moduleType =>
        {
            var type = module1.GetType();
            return type.GetProperty(propertyName);
        });

        if (propertyInfo == null)
        {
            config = null;
            return false;
        }

        config = (propertyInfo.GetValue(module) as TConfig)!;
        return true;
    }
}
