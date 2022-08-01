// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Tests.Queries;

public class UserDetailQury
{
    public Guid? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore]
    public int Age { get; set; }

    public DateTime? DelTime { get; set; }

    public List<string>? Tags { get; set; }

    public UserDetailQury()
    {
        this.Id = Guid.NewGuid();
    }

    public UserDetailQury(string name, params string[] tags) : this(name, tags.ToList())
    {
    }

    public UserDetailQury(string name, List<string> tags)
    {
        Name = name;
        Tags = tags;
    }
}
