// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation;

public interface IIsolationConfigurationProvider
{
    TModuleConfig? GetModuleConfig<TModuleConfig>(string sectionName) where TModuleConfig : class;
}
