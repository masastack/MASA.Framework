// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public interface IMasaFactory<TService> where TService : class
{
    TService Create();

    TService Create(string name);
}
