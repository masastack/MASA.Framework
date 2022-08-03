// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EntityFrameworkCore.Internal;

internal class NullDisposable : IDisposable
{
    public static NullDisposable Instance { get; } = new();

    public void Dispose()
    {
    }
}
