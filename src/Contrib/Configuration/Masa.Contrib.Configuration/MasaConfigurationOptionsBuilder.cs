// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.ConfigurationApi.Dcc")]
[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.Tests")]

namespace Masa.Contrib.Configuration;

public class MasaConfigurationOptionsBuilder
{
    public IServiceCollection Services { get; }

    public IEnumerable<Assembly>? Assemblies { get; set; }

    public bool EnableAutoMapOptions { get; set; } = true;

    private readonly List<ConfigurationRelationOptions> _registrationOptions = new();

    /// <summary>
    /// Automatically map the Options collection
    /// </summary>
    public IReadOnlyList<ConfigurationRelationOptions> RegistrationOptions => _registrationOptions;

    /// <summary>
    /// Local Configuration Builder Delegate
    /// </summary>
    public Action<IConfigurationBuilder, IServiceProvider>? ConfigurationBuilderAction { get; set; }

    public MasaConfigurationOptionsBuilder(IServiceCollection services) => Services = services;

    public IEnumerable<Assembly> GetAssemblies() => Assemblies ?? MasaApp.GetAssemblies();

    internal void AddRegistrationOptions(ConfigurationRelationOptions relationOptions)
        => ConfigurationUtils.AddRegistrationOptions(GetRegistrationOptions(), relationOptions);

    public List<ConfigurationRelationOptions> GetRegistrationOptions() => _registrationOptions;
}
