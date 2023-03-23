﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation;

/// <summary>
/// Isolation Configuration Provider
/// </summary>
public interface IIsolationConfigProvider
{
    /// <summary>
    /// Get the configuration of the specified SectionName and the specified Name under the current environment or tenant
    /// </summary>
    /// <param name="sectionName"></param>
    /// <param name="name"></param>
    /// <typeparam name="TComponentConfig"></typeparam>
    /// <returns></returns>
    TComponentConfig? GetComponentConfig<TComponentConfig>(string sectionName, string name = "") where TComponentConfig : class;

    /// <summary>
    /// Get the configuration collection of the specified SectionName and the specified Name
    /// </summary>
    /// <param name="sectionName"></param>
    /// <param name="name"></param>
    /// <typeparam name="TComponentConfig"></typeparam>
    /// <returns></returns>
    List<TComponentConfig> GetComponentConfigs<TComponentConfig>(string sectionName, string name = "") where TComponentConfig : class;
}
