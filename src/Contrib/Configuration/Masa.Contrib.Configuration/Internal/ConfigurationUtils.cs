// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal static class ConfigurationUtils
{
    public static void AddAutoMapOptions(List<ConfigurationRelationOptions> relationOptionsCollection,
        ConfigurationRelationOptions configurationRelations)
    {
        if (relationOptionsCollection.Any(relation =>
                relation.ParentSection == configurationRelations.ParentSection &&
                relation.SectionType == configurationRelations.SectionType &&
                relation.Section == configurationRelations.Section &&
                relation.OptionsName == configurationRelations.OptionsName &&
                relation.ObjectType == configurationRelations.ObjectType))
            throw new MasaException("The current section already has a configuration");

        relationOptionsCollection.Add(configurationRelations);
    }

    /// <summary>
    /// Get current environment information (Multi-environment not enabled)
    /// Priority: environment variables > appsettings.json > Production
    /// </summary>
    /// <returns></returns>
    public static string GetEnvironmentWhenDisableMultiEnvironment()
    {
        var environment = Environment.GetEnvironmentVariable(ConfigurationConstant.ENVIRONMENT_VARIABLE_NAME);
        if (!string.IsNullOrWhiteSpace(environment))
            return environment;

        var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        configurationBuilder.AddEnvironmentVariables();
        var configurationRoot = configurationBuilder.Build();
        environment= configurationRoot.GetSection(ConfigurationConstant.ENVIRONMENT_VARIABLE_NAME).Value;
        if (!environment.IsNullOrWhiteSpace())
            return environment;

        return ConfigurationConstant.DEFAULT_ENVIRONMENT;
    }

    private static readonly List<Type> SkipAutoMapTypes = new()
    {
        typeof(ConfigurationAutoMapOptions)
    };

    public static bool IsSkipAutoOptions(Type optionsType)
    {
        return SkipAutoMapTypes.Contains(optionsType);
    }
}
