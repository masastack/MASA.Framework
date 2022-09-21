// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EFCore.Tests.Models;

public class Address : ISoftDelete
{
    public int Id { get; set; }

    public string City { get; set; } = default!;

    public string Street { get; set; } = default!;

    public LogItem LastLog { get; set; }

    public Student Student { get; set; }

    public bool IsDeleted { get; set; }
}
