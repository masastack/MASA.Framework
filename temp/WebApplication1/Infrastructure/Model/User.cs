// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace WebApplication1.Infrastructure.Model;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public User()
    {
        Id = Guid.NewGuid();
    }
}
