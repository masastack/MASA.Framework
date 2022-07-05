// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EF;

public static class MasaDbContextOptionsBuilderExtensions
{
    private static readonly List<Type> _types = new();

    public static MasaDbContextBuilder UseFilter(
        this MasaDbContextBuilder masaDbContextOptionsBuilder,
        Action<FilterOptions>? options = null)
        => masaDbContextOptionsBuilder.UseFilterCore(false, options);

    public static MasaDbContextBuilder UseTestFilter(
        this MasaDbContextBuilder masaDbContextOptionsBuilder,
        Action<FilterOptions>? options = null)
        => masaDbContextOptionsBuilder.UseFilterCore(true, options);

    private static MasaDbContextBuilder UseFilterCore(
        this MasaDbContextBuilder masaDbContextOptionsBuilder,
        bool isTest,
        Action<FilterOptions>? options = null)
    {
        var filterOptions = new FilterOptions();
        options?.Invoke(filterOptions);

        masaDbContextOptionsBuilder.Services.TryAddScoped(typeof(DataFilter<>));
        masaDbContextOptionsBuilder.Services.TryAddScoped<IDataFilter, DataFilter>();

        if (filterOptions.EnableSoftDelete) masaDbContextOptionsBuilder.UseSoftDelete(isTest);

        return masaDbContextOptionsBuilder;
    }

    private static void UseSoftDelete(this MasaDbContextBuilder masaDbContextOptionsBuilder, bool isTest = false)
    {
        if (!isTest)
        {
            if (_types.Any(type => masaDbContextOptionsBuilder.DbContextType == type))
                return;

            _types.Add(masaDbContextOptionsBuilder.DbContextType);
        }

        var masaDbContextOptionsType = typeof(MasaDbContextOptions<>).MakeGenericType(masaDbContextOptionsBuilder.DbContextType);
        var softDeleteSaveChangesFilterType =
            typeof(SoftDeleteSaveChangesFilter<,>).MakeGenericType(
                masaDbContextOptionsBuilder.DbContextType,
                masaDbContextOptionsBuilder.UserIdType);
        var constructorInfo = softDeleteSaveChangesFilterType.GetConstructors().FirstOrDefault()!;
        var invokeDelegate = InstanceBuilder.CreateInstanceDelegate(constructorInfo);

        masaDbContextOptionsBuilder.Services.Add(
            new ServiceDescriptor(typeof(ISaveChangesFilter),
                serviceProvider =>
                {
                    var instance = invokeDelegate.Invoke(
                        serviceProvider.GetRequiredService(masaDbContextOptionsType),
                        serviceProvider.GetRequiredService(masaDbContextOptionsBuilder.DbContextType),
                        serviceProvider.GetService<IUserContext>());
                    return instance;
                },
                ServiceLifetime.Scoped));

        masaDbContextOptionsBuilder.EnableSoftDelete = true;
    }
}
