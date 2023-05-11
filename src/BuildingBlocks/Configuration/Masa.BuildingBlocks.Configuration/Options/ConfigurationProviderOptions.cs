// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

public class ConfigurationProviderOptions
{
    public IConfigurationBuilder ConfigurationBuilder { get; }

    public IServiceProvider ServiceProvider { get; }

    public SectionTypes SectionType { get; set; }

    public ConfigurationProviderOptions(
        IConfigurationBuilder configurationBuilder,
        IServiceProvider serviceProvider,
        SectionTypes sectionType)
    {
        ConfigurationBuilder = configurationBuilder;
        ServiceProvider = serviceProvider;
        SectionType = sectionType;
    }
}
