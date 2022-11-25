// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.IntegrationTests.Domain.Entities;

public class Country : ValueObject
{
    public string Name { get; set; }

    public Country()
    {
        Name = string.Empty;
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Name;
    }
}
