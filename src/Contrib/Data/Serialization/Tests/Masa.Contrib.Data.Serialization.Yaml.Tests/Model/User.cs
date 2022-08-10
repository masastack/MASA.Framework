// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Serialization.Yaml.Tests.Model;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public User()
    {

    }

    public User(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
