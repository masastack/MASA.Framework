// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EF.CustomRepository.Tests.Entities;

public class User : AggregateRoot<Guid>
{
    public string Name { get; set; }

    public bool Gender { get; set; }
}
