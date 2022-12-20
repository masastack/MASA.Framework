// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

#pragma warning disable S2342
// ReSharper disable once InconsistentNaming
public enum DESEncryptType
{
    /// <summary>
    /// original DES encryption
    /// </summary>
    Normal,

    /// <summary>
    /// Easy to transfer in browser
    /// </summary>
    Improved
}
#pragma warning restore S2342
