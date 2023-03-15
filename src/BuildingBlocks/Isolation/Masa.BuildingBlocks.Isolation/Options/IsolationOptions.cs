// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public class IsolationOptions
{
    public string SectionName { get; set; }

    /// <summary>
    /// isolation status
    /// default: false
    /// </summary>
    public bool Enable { get; set; }
}

public class IsolationOptions<TModule>
{
    public List<IsolationConfigurationOptions<TModule>> Data { get; set; } = new();
}
