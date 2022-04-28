// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contribs.Ddd.Domain.Entities.Tests;

public class Users : AggregateRoot<Guid>
{
    public string Name { get; set; }
}

