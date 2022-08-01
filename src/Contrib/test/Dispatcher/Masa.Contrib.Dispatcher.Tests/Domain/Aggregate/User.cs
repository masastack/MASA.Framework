// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Tests.Domain.Aggregate;

public class User : AggregateRoot<Guid>
{
    public string Name { get; set; }

    public int Age { get; set; }

    public User()
    {
        Id = Guid.NewGuid();
    }
}
