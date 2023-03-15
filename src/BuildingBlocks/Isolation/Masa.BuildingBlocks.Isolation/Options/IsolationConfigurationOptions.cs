// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public class IsolationConfigurationOptions
{
    public string TenantId { get; set; }

    public string Environment { get; set; }

    /// <summary>
    /// Used to control the configuration with the highest score when multiple configurations are satisfied. The default score is 100
    /// </summary>
    public int Score { get; set; } = 100;
}

public class IsolationConfigurationOptions<TModule>: IsolationConfigurationOptions
{
    public TModule Module { get; set; }
}
