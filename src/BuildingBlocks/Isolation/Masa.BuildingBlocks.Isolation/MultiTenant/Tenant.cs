// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation.MultiTenant;

public class Tenant
{
    public string Id { get; set; }

    public Tenant(object id) : this(id?.ToString() ?? throw new ArgumentNullException(nameof(id)))
    {
    }

    public Tenant(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        Id = id;
    }
}
