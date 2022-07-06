// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Ddd.Domain.Entities.Tests.Events;

namespace Masa.Contrib.Ddd.Domain.Entities.Tests;

public class User : AggregateRoot<Guid>
{
    public string Name { get; set; }

    public User()
    {
        Id = Guid.NewGuid();
    }

    public User(Guid id, string name)
    {
        Id = id;
        Name = name;
        this.AddDomainEvent(new RegisterUserDomainEvent()
        {
            Id = Id,
            Name = name
        });
    }

    public void UpdateName(string name)
    {
        this.Name = name;
        this.AddDomainEvent(new UpdateUserDomainEvent()
        {
            Id = Id,
            Name = name
        });
    }
}
