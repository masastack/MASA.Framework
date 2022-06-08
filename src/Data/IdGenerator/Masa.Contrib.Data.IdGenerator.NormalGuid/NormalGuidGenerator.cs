// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.NormalGuid;

public class NormalGuidGenerator : IGuidGenerator
{
    public Guid Create() => Guid.NewGuid();
}


