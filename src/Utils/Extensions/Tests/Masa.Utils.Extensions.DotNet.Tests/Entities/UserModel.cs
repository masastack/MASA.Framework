// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.DotNet.Tests.Entities;

public class User
{
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    public int Age { get; set; }

    public int? Age2 { get; set; }

    public bool Gender { get; set; }

    public bool? Gender2 { get; set; }

    public List<string> Tags { get; set; }

    public int[] UserRoles { get; set; }

    public UserClaimType UserClaimType { get; set; }

    public object Avatar { get; set; }

    public object Avatar2 { get; set; }

    public DateTime CreateTime { get; set; }
}
