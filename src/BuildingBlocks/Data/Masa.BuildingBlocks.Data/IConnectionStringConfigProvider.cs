// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

/// <summary>
/// This is an internal API backing the ConnectionString infrastructure and is not subject to the same compatibility standards as the public API. It may be changed or removed without notice
/// Database connection string configuration provider (non-isolated mode)
/// </summary>
public interface IConnectionStringConfigProvider
{
    Dictionary<string, string> GetConnectionStrings();
}
