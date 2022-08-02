// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.Internal;

public enum DaprProcessStatus
{
    Starting = 1,
    Started,
    Stopping,
    Stopped,
    Restarting
}
