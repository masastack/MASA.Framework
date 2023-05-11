// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

/// <summary>
/// Configure the cache, and get MasaConfiguration according to the storage environment and node type
/// </summary>
[ExcludeFromCodeCoverage]
internal class MasaConfigurationOptionsCache : MemoryCache<(string Name, SectionTypes SectionType), IMasaConfiguration>
{

}
