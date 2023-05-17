// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal static class MasaConfigurationOptionsBuilderExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="masaConfigurationOptionsBuilder"></param>
    public static void AddOptions(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder)
    {
        masaConfigurationOptionsBuilder.BuildOptionsRelations();

        AddOptions(masaConfigurationOptionsBuilder.Services);
        AddOrUpdateOptionsMonitor(masaConfigurationOptionsBuilder.Services);
    }

    /// <summary>
    /// Build a collection of option patterns to be registered
    /// </summary>
    /// <param name="masaConfigurationOptionsBuilder"></param>
    /// <exception cref="MasaException"></exception>
    public static void BuildOptionsRelations(this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder)
    {
        var autoMapOptionsAll = new List<ConfigurationRelationOptions>(masaConfigurationOptionsBuilder.AutoMapOptionsByManual);
        if (masaConfigurationOptionsBuilder.EnableAutoMapOptions)
        {
            var autoMapOptionsByAutomatic = GetAutoMasaOptionsByAutomatic(masaConfigurationOptionsBuilder.GetAssemblies());

            foreach (var item in autoMapOptionsByAutomatic)
            {
                ConfigurationUtils.AddAutoMapOptions(autoMapOptionsAll, item);
            }
        }

        // List of documented support option patterns
        masaConfigurationOptionsBuilder.Services.Configure<ConfigurationAutoMapOptions>(options =>
        {
            foreach (var item in autoMapOptionsAll)
            {
                ConfigurationUtils.AddAutoMapOptions(options.Data, item);
            }
        });
    }

    private static List<ConfigurationRelationOptions> GetAutoMasaOptionsByAutomatic(IEnumerable<Assembly> assemblies)
    {
        var optionTypes = GetAutoOptionsTypes();
        var data = new List<ConfigurationRelationOptions>();

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
            ConfigurationUtils.AddAutoMapOptions(data, configurationRelationOptions);
        });

        return data;

        List<Type> GetAutoOptionsTypes()
        {
            return assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type != typeof(IMasaOptionsConfigurable) &&
                    type is { IsAbstract: false, IsGenericType: false } &&
                    typeof(IMasaOptionsConfigurable).IsAssignableFrom(type))
                .ToList();
        }
    }

    private static void AddOptions(IServiceCollection services)
    {
        services.Replace(new ServiceDescriptor(typeof(IConfigureNamedOptions<>), typeof(MasaConfigureNamedOptions<>), ServiceLifetime.Transient));
        services.Replace(new ServiceDescriptor(typeof(IConfigureOptions<>), typeof(MasaConfigureNamedOptions<>), ServiceLifetime.Transient));
        services.AddTransient(typeof(MasaConfigureNamedOptions<>));
        services.AddTransient<MasaConfigureNamedOptionsProvider, MasaConfigureNamedOptionsProvider>();
        services.Replace(new ServiceDescriptor(typeof(IOptions<>), typeof(MasaUnnamedOptionsManager<>), ServiceLifetime.Transient));
        services.AddTransient<MasaUnnamedOptionsProvider>();
        services.AddTransient(typeof(MasaUnnamedOptionsProvider<>));
        services.AddSingleton(typeof(MasaUnnamedOptionsCache<>));
        services.AddSingleton(typeof(UnnamedOptionsManager<>));
    }

    private static void AddOrUpdateOptionsMonitor(IServiceCollection services)
    {
        services.Replace(new ServiceDescriptor(typeof(IOptionsMonitor<>), typeof(MasaOptionsMonitor<>), ServiceLifetime.Transient));
        services.AddTransient<MasaOptionsMonitorProvider>();
        services.AddSingleton(typeof(OptionsMonitor<>));
    }
}
