// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Identity.Internal;

internal class DisposeAction : IDisposable
{
    private readonly Action _action;

    public DisposeAction(Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        _action = action;
    }

    public void Dispose() => _action();
}
