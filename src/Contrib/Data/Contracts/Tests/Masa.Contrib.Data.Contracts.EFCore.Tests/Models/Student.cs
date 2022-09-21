// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EFCore.Tests.Models;

public class Student : ISoftDelete
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public int Age { get; set; }

    public bool IsDeleted { get; private set; } = default!;

    public Address Address { get; set; } = default!;

    public List<Hobby> Hobbies { get; set; } = default!;
}
