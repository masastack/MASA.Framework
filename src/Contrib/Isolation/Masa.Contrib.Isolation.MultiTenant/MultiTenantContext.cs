// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiTenant;

public class MultiTenantContext : IMultiTenantContext, IMultiTenantSetter
{
    public Tenant? CurrentTenant { get; private set; }

    public void SetTenant(Tenant? tenant) => CurrentTenant = tenant;
}
