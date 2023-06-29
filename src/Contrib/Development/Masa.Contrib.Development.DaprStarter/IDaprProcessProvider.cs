// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

public interface IDaprProcessProvider
{
    List<DaprRuntimeOptions> GetDaprList(string fileName, string appId, out bool isException);

    Process DaprStart(
        string fileName,
        string arguments,
        bool createNoWindow,
        Action<object?, DataReceivedEventArgs> outputDataReceivedAction,
        Action exitAction);

    void DaprStop(string fileName, string appId);
}
