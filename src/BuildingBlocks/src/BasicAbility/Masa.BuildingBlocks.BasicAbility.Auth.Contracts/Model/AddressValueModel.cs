// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Auth.Contracts.Model;

public class AddressValueModel
{
    public string Address { get; set; }

    public string? ProvinceCode { get; set; }

    public string? CityCode { get; set; }

    public string? DistrictCode { get; set; }

    public AddressValueModel()
    {
        Address = "";
    }

    public AddressValueModel(string address, string? provinceCode, string? cityCode, string? districtCode)
    {
        Address = address;
        ProvinceCode = provinceCode;
        CityCode = cityCode;
        DistrictCode = districtCode;
    }

    public override string ToString()
    {
        return Address;
    }
}
