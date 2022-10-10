// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr;

public interface IDaprProvider
{
    List<DaprRuntimeOptions> GetDaprList(string appId);

    void DaprStop(string appId);

    bool IsExist(string appId);
}
