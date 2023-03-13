// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.IdGenerator.NormalGuid.Tests")]

namespace Masa.Contrib.Data.IdGenerator.NormalGuid;

internal class NormalGuidGenerator : IdGeneratorBase<Guid>, IGuidGenerator
{
    public override Guid NewId() => Guid.NewGuid();
}
