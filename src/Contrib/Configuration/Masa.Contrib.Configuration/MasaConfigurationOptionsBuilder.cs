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

    internal List<ConfigurationRelationOptions> AutoMapOptionsByManual = new();

    /// <summary>
    /// Local Configuration Builder Delegate
    /// </summary>
    public Action<IConfigurationBuilder, IServiceProvider>? ConfigurationBuilderAction { get; set; }

    public MasaConfigurationOptionsBuilder(IServiceCollection services) => Services = services;

    public IEnumerable<Assembly> GetAssemblies() => Assemblies ?? MasaApp.GetAssemblies();
}
