// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal;

internal class InitializeServiceProvider : IInitializeServiceProvider
{
    private bool _initialized;

    public bool IsInitialize => _initialized;

    public InitializeServiceProvider()
    {
        _initialized = false;
    }

    public void Initialize()
    {
        _initialized = true;
    }

    public void Reset()
    {
        _initialized = false;
    }
}
