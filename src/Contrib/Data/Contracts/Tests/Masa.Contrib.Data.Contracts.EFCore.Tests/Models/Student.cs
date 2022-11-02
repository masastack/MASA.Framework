// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EFCore.Tests.Models;

public class Student : IAuditEntity<int>, ISoftDelete
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public int Age { get; set; }

    public bool IsDeleted { get; private set; } = default!;

    public Address Address { get; set; } = default!;

    public List<Hobby> Hobbies { get; set; } = default!;

    public int Creator { get; private set; }

    public DateTime CreationTime { get; private set; }

    public int Modifier { get; private set; }

    public DateTime ModificationTime { get; private set; }

    public IEnumerable<(string Name, object Value)> GetKeys()
    {
        return new List<(string Name, object Value)>()
        {
            (nameof(Id), Id)
        };
    }
}
