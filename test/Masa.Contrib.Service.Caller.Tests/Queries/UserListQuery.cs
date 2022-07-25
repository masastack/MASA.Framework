// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Tests.Queries;

public class UserListQuery
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore]
    public int Age { get; set; }

    public UserListQuery(string name)
    {
        Name = name;
    }
}
