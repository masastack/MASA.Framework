// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Extensions;

public class IdGeneratorHelper
{
    private static IIdGenerator? _idGenerator;

    private static IIdGenerator IdGenerator =>
        _idGenerator ??= ServiceCollectionExtensions.Services.BuildServiceProvider().GetRequiredService<IIdGenerator>();

    public static long Generate() => IdGenerator.Generate();
}
