// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests.Domain.ValueObjects;

public class AddressItem
{
    public string Province { get; set; }

    public string City { get; set; }

    public string Address { get; set; }

    public AddressItem(string fullAddress) : this(fullAddress.Split(',')[0], fullAddress.Split(',')[1], fullAddress.Split(',')[2])
    {

    }

    public AddressItem(string province, string city, string address)
    {
        Province = province;
        City = city;
        Address = address;
    }
}
