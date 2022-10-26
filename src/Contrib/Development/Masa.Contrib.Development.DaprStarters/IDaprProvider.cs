// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarters;

public interface IDaprProvider
{
    List<DaprRuntimeOptions> GetDaprList(string appId);

    Process DaprStart(string arguments,
        bool createNoWindow,
        Action<object?, DataReceivedEventArgs> outputDataReceivedAction,
        Action exitAction);

    void DaprStop(string appId);

    bool IsExist(string appId);
}
