// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal static class MasaConfigurationOptionsBuilderExtensions
{
    public static void AddOptions(this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder)
    {
        var relations = masaConfigurationOptionsBuilder.BuildOptionsRelations();

        AddOrUpdateOptions(masaConfigurationOptionsBuilder.Services, relations);

        AddOrUpdateOptionsSnapshot(masaConfigurationOptionsBuilder.Services, relations);

        AddOrUpdateOptionsMonitor(masaConfigurationOptionsBuilder.Services, relations);
    }

    /// <summary>
    /// Build a collection of option patterns to be registered
    /// </summary>
    /// <param name="masaConfigurationOptionsBuilder"></param>
    /// <exception cref="MasaException"></exception>
    public static IReadOnlyList<ConfigurationRelationOptions> BuildOptionsRelations(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder)
    {
        if (!masaConfigurationOptionsBuilder.EnableAutoMapOptions)
            return masaConfigurationOptionsBuilder.RegistrationOptions;

        var optionTypes = GetAutoOptionsTypes(masaConfigurationOptionsBuilder.GetAssemblies());

        optionTypes.ForEach(optionType =>
        {
            var constructorInfo = optionType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(info => info.GetParameters().Length == 0);

            if (constructorInfo == null)
                throw new MasaException($"[{optionType.Name}] must have a parameterless constructor");

            var option = (IMasaOptionsConfigurable)Activator.CreateInstance(optionType, !constructorInfo.IsPublic)!;

            var configurationRelationOptions = new ConfigurationRelationOptions
            {
                SectionType = option.GetSectionType(),
                ParentSection = option.GetParentSection(),
                Section = option.GetSection() ?? optionType.Name,
                ObjectType = optionType,
                OptionsName = option.GetOptionsName()
            };
            masaConfigurationOptionsBuilder.AddRegistrationOptions(configurationRelationOptions);
        });

        return masaConfigurationOptionsBuilder.RegistrationOptions;

        List<Type> GetAutoOptionsTypes(IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type != typeof(IMasaOptionsConfigurable) &&
                    type is { IsAbstract: false, IsGenericType: false } &&
                    typeof(IMasaOptionsConfigurable).IsAssignableFrom(type))
                .ToList();
        }
    }

    private static void AddOrUpdateOptions(IServiceCollection services,
        IReadOnlyList<ConfigurationRelationOptions> relationOptions)
    {
        var singletonServiceGenericType = typeof(SingletonService<>);
        var scopedServiceGenericType = typeof(ScopedService<>);
        var masaUnnamedOptionsProviderGenericType = typeof(MasaUnnamedOptionsProvider<>);
        var masaUnnamedOptionsCacheGenericType = typeof(MasaUnnamedOptionsCache<>);

        foreach (var objectType in relationOptions.Select(options => options.ObjectType).Distinct())
        {
            var masaUnnamedOptionsCacheType = masaUnnamedOptionsCacheGenericType.MakeGenericType(objectType);
            var masaUnnamedOptionsProviderType = masaUnnamedOptionsProviderGenericType.MakeGenericType(objectType);

            var singleMasaUnnamedOptionsProviderType = singletonServiceGenericType.MakeGenericType(masaUnnamedOptionsProviderType);
            var scopedMasaUnnamedOptionsProviderType = scopedServiceGenericType.MakeGenericType(masaUnnamedOptionsProviderType);

            services.AddSingleton(singleMasaUnnamedOptionsProviderType, serviceProvider =>
            {
                var masaUnnamedOptionsCache = serviceProvider.GetService(masaUnnamedOptionsCacheType);
                var masaUnnamedOptionsProvider = Activator.CreateInstance(masaUnnamedOptionsProviderType, masaUnnamedOptionsCache);
                return Activator.CreateInstance(singleMasaUnnamedOptionsProviderType, masaUnnamedOptionsProvider)!;
            });

            services.AddScoped(scopedMasaUnnamedOptionsProviderType, serviceProvider =>
            {
                var masaUnnamedOptionsCache = serviceProvider.GetService(masaUnnamedOptionsCacheType);
                var masaUnnamedOptionsProvider = Activator.CreateInstance(masaUnnamedOptionsProviderType, masaUnnamedOptionsCache);
                return Activator.CreateInstance(scopedMasaUnnamedOptionsProviderType, masaUnnamedOptionsProvider)!;
            });

            services.AddTransient(masaUnnamedOptionsProviderType, serviceProvider =>
            {
                var enableMultiEnvironment = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>().Value.EnableMultiEnvironment;
                return enableMultiEnvironment ?
                    (serviceProvider.GetRequiredService(scopedMasaUnnamedOptionsProviderType) as LifetimeServiceBase)!.GetService() :
                    (serviceProvider.GetRequiredService(singleMasaUnnamedOptionsProviderType) as LifetimeServiceBase)!.GetService();
            });
        }

        services.AddSingleton(typeof(UnnamedOptionsManager<>));
        services.AddTransient(masaUnnamedOptionsCacheGenericType);
        services.Replace(new ServiceDescriptor(typeof(IOptions<>), typeof(MasaUnnamedOptionsManager<>), ServiceLifetime.Transient));
    }

    private static void AddOrUpdateOptionsSnapshot(IServiceCollection services, IReadOnlyList<ConfigurationRelationOptions> relationOptions)
    {
        var serviceGenericType = typeof(IConfigureNamedOptions<>);
        var configureOptionsType = typeof(IConfigureOptions<>);
        var implementationGenericType = typeof(MasaConfigureNamedOptions<>);

        var configurationRelationOptions = relationOptions.ToDictionary(options => (options.ObjectType, Name: options.OptionsName), options => options);

        foreach (var objectType in relationOptions.Select(options => options.ObjectType).Distinct())
        {
            var serviceType = serviceGenericType.MakeGenericType(objectType);
            var implementationType = implementationGenericType.MakeGenericType(objectType);

            services.TryAddTransient(configureOptionsType.MakeGenericType(objectType),
                serviceProvider => serviceProvider.GetRequiredService(serviceType));
            services.TryAddTransient(serviceType, serviceProvider =>
            {
                var implementation = Activator.CreateInstance(implementationType, serviceProvider, configurationRelationOptions);
                return implementation!;
            });
        }
    }

    private static void AddOrUpdateOptionsMonitor(IServiceCollection services, IReadOnlyList<ConfigurationRelationOptions> relationOptions)
    {
        services.Replace(new ServiceDescriptor(typeof(IOptionsMonitor<>), typeof(MasaOptionsMonitor<>), ServiceLifetime.Transient));
        services.AddSingleton<MasaOptionsMonitorProvider>(_ => new MasaOptionsMonitorProvider(relationOptions));
        services.AddSingleton(typeof(OptionsMonitor<>));
    }
}
