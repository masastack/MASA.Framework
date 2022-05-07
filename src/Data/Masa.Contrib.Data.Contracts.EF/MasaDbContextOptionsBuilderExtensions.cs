// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EF;

public static class MasaDbContextOptionsBuilderExtensions
{
    private static readonly List<Type> _types = new();

    public static MasaDbContextOptionsBuilder UseFilter(
        this MasaDbContextOptionsBuilder masaDbContextOptionsBuilder,
        Action<FilterOptions>? options = null)
    {
        var filterOptions = new FilterOptions();
        options?.Invoke(filterOptions);

        masaDbContextOptionsBuilder.Services.TryAddScoped(typeof(DataFilter<>));
        masaDbContextOptionsBuilder.Services.TryAddScoped<IDataFilter, DataFilter>();

        if (filterOptions.EnableSoftDelete) masaDbContextOptionsBuilder.UseSoftDelete();

        return masaDbContextOptionsBuilder;
    }

    private static void UseSoftDelete(this MasaDbContextOptionsBuilder masaDbContextOptionsBuilder)
    {
        if (_types.Any(type => masaDbContextOptionsBuilder.DbContextType == type))
            return;

        _types.Add(masaDbContextOptionsBuilder.DbContextType);

        var masaDbContextOptionsType = typeof(MasaDbContextOptions<>).MakeGenericType(masaDbContextOptionsBuilder.DbContextType);
        var softDeleteSaveChangesFilterType =
            typeof(SoftDeleteSaveChangesFilter<>).MakeGenericType(masaDbContextOptionsBuilder.DbContextType);
        var constructorInfo = softDeleteSaveChangesFilterType.GetConstructors().FirstOrDefault()!;
        var invokeDelegate = InstanceBuilder.CreateInstanceDelegate(constructorInfo);

        masaDbContextOptionsBuilder.Services.TryAdd(
            new ServiceDescriptor(typeof(ISaveChangesFilter),
                serviceProvider =>
                {
                    var instance= invokeDelegate.Invoke(
                        serviceProvider.GetRequiredService(masaDbContextOptionsType),
                        serviceProvider.GetRequiredService(masaDbContextOptionsBuilder.DbContextType));
                    return instance;
                },
                ServiceLifetime.Scoped));

        masaDbContextOptionsBuilder.EnableSoftDelete = true;
    }
}
