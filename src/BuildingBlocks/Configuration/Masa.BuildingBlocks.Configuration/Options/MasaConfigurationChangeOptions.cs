// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Options;

/// <summary>
///
/// </summary>
public class MasaConfigurationChangeOptions
{
    public IConfiguration Configuration { get; }

    public string Environment { get; }

    public MasaConfigurationChangeOptions(IConfiguration configuration, string environment)
    {
        Configuration = configuration;
        Environment = environment;
    }
}
