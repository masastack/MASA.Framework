// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Options;

public class MasaDbContextConfigurationOptions
{
    public string ConnectionString { get; }

    public MasaDbContextConfigurationOptions(string connectionString) => ConnectionString = connectionString;
}
