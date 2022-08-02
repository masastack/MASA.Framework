// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests.Domain.Aggregates.Users;

public class User
{
    public string Name { get; set; }

    public int Age { get; set; }

    public string Description { get; set; }

    public DateTime Birthday { get; set; }

    public AddressItem Hometown { get; set; }


    public User()
    {

    }
    public User(string name)
    {
        Name = name;
    }

    public User(string name, int age, string description, DateTime birthday)
        : this(name)
    {
        Age = age;
        Description = description;
        Birthday = birthday;
    }

    public User(string name, int age, string description, DateTime birthday, AddressItem hometown)
        : this(name, age, description, birthday)
    {
        Hometown = hometown;
    }
}
