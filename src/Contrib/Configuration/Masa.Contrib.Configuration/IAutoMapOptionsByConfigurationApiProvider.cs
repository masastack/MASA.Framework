// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

/// <summary>
/// The data source used to supplement and improve the automatic mapping is the incomplete AppId information in ConfigurationAPI
/// </summary>
public interface IAutoMapOptionsByConfigurationApiProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionsType"></param>
    /// <param name="optionsName"></param>
    /// <param name="autoMapOptions"></param>
    /// <returns></returns>
    bool TryGetAutoMapOptions(Type optionsType, string optionsName, [NotNullWhen(true)] out ConfigurationRelationOptions? autoMapOptions);
}
