// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Isolation;

public interface IMultiTenantSetter
{
    void SetTenant(Tenant? tenant);
}

[Obsolete("Use IMultiTenantSetter instead")]
public interface ITenantSetter
{
    void SetTenant(Tenant? tenant);
}
