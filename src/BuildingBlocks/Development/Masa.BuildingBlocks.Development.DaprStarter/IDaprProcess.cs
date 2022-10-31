// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Development.DaprStarter;

public interface IDaprProcess : IDisposable
{
    void Start();

    void Stop();
}
