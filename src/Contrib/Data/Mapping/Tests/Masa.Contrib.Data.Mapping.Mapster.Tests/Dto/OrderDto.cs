// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests;

public class OrderDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public AddressItemDto Address { get; set; }
}
