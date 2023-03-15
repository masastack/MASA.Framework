// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation;

public interface IIsolationConfigurationProvider<TModuleConfig> where TModuleConfig : class
{
    bool TryGetModule(string sectionName, [NotNullWhen(true)] out TModuleConfig? module);
}
