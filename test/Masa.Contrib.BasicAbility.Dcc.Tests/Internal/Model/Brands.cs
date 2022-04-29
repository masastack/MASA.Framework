// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Dcc.Tests.Internal.Model;

internal class Brands
{
    public Guid Id { get; init; }

    public string Name { get; set; }

    public Brands()
        => this.Id = Guid.NewGuid();

    public Brands(string Name) : this()
        => this.Name = Name;
}
