// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

public class UserModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public int? Age2 { get; set; }

    public bool Gender { get; set; }

    public bool? Gender2 { get; set; }

    public List<string> Tags { get; set; }

    public int[] UserRoles { get; set; }

    public UserClaimType UserClaimType { get; set; }

    public object Avatar { get; set; }

    public DateTime CreateTime { get; set; }
}
