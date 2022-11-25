// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.IntegrationTests.Domain.Entities;

public class Address : ValueObject
{
    public Country Country { get; set; }

    public string Street { get; set; }

    public string City { get; set; }

    public string State { get; set; }


    public string ZipCode { get; set; }

    public Address()
    {
        Street = string.Empty;
        City = string.Empty;
        State = string.Empty;
        ZipCode = string.Empty;
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return ZipCode;
    }
}
