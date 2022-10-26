// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Development.DaprStarters;

internal enum DaprProcessStatus
{
    Starting = 1,
    Started,
    Stopping,
    Stopped,
    Restarting
}
