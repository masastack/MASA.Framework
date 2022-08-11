// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Serialization.Yaml.Tests.Model;

public class User
{
    public decimal Age { get; set; }

    public string Name { get; set; }

    public User()
    {

    }

    public User(decimal age, string name)
    {
        Age = age;
        Name = name;
    }
}
