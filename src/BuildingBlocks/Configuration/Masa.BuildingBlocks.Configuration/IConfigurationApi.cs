// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public interface IConfigurationApi
{
    public IConfiguration Get(string appId);

    // string GetDefaultAppId();
}
