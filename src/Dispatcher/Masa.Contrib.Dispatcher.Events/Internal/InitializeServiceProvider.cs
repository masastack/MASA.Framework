// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal;

internal class InitializeServiceProvider : IInitializeServiceProvider
{
    private bool _state;

    public bool IsInitialize => _state;

    public InitializeServiceProvider()
    {
        _state = false;
    }

    public void Initialize()
    {
        _state = true;
    }

    public void Reset()
    {
        _state = false;
    }
}
