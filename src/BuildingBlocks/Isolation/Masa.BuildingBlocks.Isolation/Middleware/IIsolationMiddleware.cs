// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Isolation.Middleware;

public interface IIsolationMiddleware
{
    Task HandleAsync();
}
