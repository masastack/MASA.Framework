// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public interface IMasaConfiguration
{
    public IConfiguration Local { get; }

    public IConfigurationApi? ConfigurationApi { get; }

    IConfiguration GetConfiguration(SectionTypes sectionType);
}
