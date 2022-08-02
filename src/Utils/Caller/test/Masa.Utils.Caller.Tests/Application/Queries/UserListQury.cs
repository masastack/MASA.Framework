// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Tests.Application.Queries;

public class UserListQury
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore]
    public int Age { get; set; }

    public UserListQury(string name)
    {
        Name = name;
    }
}
