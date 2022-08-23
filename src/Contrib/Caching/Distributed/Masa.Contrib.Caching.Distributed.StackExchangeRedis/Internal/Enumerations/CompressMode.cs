// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal enum CompressMode
{
    /// <summary>
    /// no compression
    /// </summary>
    None = 1,

    /// <summary>
    /// Compress but not deserialize
    /// </summary>
    Compress,

    /// <summary>
    /// serialize and compress
    /// </summary>
    SerializeAndCompress,
}
