// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class MasaDbContextConfigurationOptions
{
    public string ConnectionString { get; }

    public MasaDbContextConfigurationOptions(string connectionString) => ConnectionString = connectionString;
}
