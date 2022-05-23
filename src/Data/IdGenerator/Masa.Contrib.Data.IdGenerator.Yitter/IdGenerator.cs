// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Yitter;

public class IdGenerator : IIdGenerator
{
    private readonly YitterIIdGenerator _IdGenInstance;

    public IdGenerator(IdGeneratorOptions idGeneratorOptions)
        => _IdGenInstance = new DefaultIdGenerator(idGeneratorOptions);

    public long Generate() => _IdGenInstance.NewLong();
}
