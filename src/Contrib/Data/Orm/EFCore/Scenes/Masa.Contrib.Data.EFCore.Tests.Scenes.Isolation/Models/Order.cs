// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore.Tests.Scenes.Isolation;

public class Order : IMultiTenant
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }
}

public class Order2 : IMultiTenant
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }
}
