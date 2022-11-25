// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.Internal;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class NullDisposable : IDisposable
{
    public static NullDisposable Instance { get; } = new();

    /// <summary>
    /// no need to release resources
    /// </summary>
    public void Dispose()
    {
    }
}
