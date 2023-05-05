// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests.Entities;

public class Hobby : ISoftDelete
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public Guid StudentId { get; private set; } = default!;

    public Student Student { get; private set; } = default!;

    public bool IsDeleted { get; private set; }

    public Hobby()
    {
        Id = Guid.NewGuid();
    }
}
