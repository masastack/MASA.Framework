// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage.Internal;

internal static class ObjectStorageClientConstant
{
    /// <summary>
    /// Used to set the default life cycle of IObjectStorageClient
    /// </summary>
    public const ServiceLifetime DEFAULT_LIFETIME = ServiceLifetime.Singleton;
}
