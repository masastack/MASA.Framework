// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests.Models;

public class User: Entity<Guid>
{
    public string Name { get; set; } = default!;

    public string Email { get; set; }

    public string PhoneNumber { get; set; } = default!;

    public int Age { get; set; }

    public string CompanyName { get; set; } = default!;
}
