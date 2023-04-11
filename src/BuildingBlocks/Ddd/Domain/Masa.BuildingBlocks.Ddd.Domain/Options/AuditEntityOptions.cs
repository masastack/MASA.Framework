// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Ddd.Domain;

public class AuditEntityOptions
{
    public Type UserIdType { get; set; }

    public AuditEntityOptions()
    {
        UserIdType = typeof(Guid);
    }
}
