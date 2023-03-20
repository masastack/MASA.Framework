// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation;

public interface IIsolationConfigProvider
{
    TModuleConfig? GetModuleConfig<TModuleConfig>(string name, string sectionName) where TModuleConfig : class;

    List<TModuleConfig> GetModuleConfigs<TModuleConfig>(string name, string sectionName) where TModuleConfig : class;
}
