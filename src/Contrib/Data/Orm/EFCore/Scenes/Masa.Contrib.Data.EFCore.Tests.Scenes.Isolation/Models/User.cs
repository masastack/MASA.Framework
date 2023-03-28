// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore.Tests.Scenes.Isolation;

public class User : IMultiTenant<string>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string TenantId { get; set; }
}
