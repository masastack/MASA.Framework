// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Tests;

public class CustomerResource
{
    public string Name { get; set; }

    public OrderResource Order { get; set; }

    public class OrderResource
    {
        public string Name { get; set; }
    }
}
