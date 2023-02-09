// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Middleware;

public interface IDisabledDeterminer
{
    bool Determiner();
}

public interface IDisabledEventDeterminer : IDisabledDeterminer
{
    bool DisabledCommand { get; }
}

public interface IDisabledRequestDeterminer : IDisabledDeterminer
{
}
