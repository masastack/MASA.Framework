// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests.Entities;

public class People : FullAggregateRoot<Guid, Guid?>
{
    public string Name { get; set; }
}

public class People2 : FullAggregateRoot<Guid, string?>
{
    public string Name { get; set; }
}
