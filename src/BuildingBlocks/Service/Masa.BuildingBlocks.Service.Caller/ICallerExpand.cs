// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public interface ICallerExpand : ICaller
{
    void ConfigRequestMessage(Func<HttpRequestMessage, Task> func);
}
