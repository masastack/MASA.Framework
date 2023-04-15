// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests.Entities;

public class Goods : FullAggregateRoot<Guid, int>
{
    public string Name { get; set; }
}

public class Goods2 : FullAggregateRoot<Guid, int?>
{
    public string Name { get; set; }
}
