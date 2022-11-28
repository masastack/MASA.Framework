// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public interface IMultiTenantContext
{
    Tenant? CurrentTenant { get; }
}

[Obsolete("Use IMultiTenantContext instead")]
public interface ITenantContext
{
    Tenant? CurrentTenant { get; }
}
