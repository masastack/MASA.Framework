// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public interface IMasaConfigurationFactory
{
    /// <summary>
    /// Get the configuration information of the specified node type
    /// When multiple environments are enabled, the obtained configuration is the configuration information in the current environment
    /// </summary>
    /// <param name="sectionType"></param>
    /// <returns></returns>
    IMasaConfiguration Create(SectionTypes sectionType);
}
