// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

/// <summary>
/// Hash algorithm encryption SHA1
/// </summary>
public class SHA1Utils : HashAlgorithmBase
{
    /// <summary>
    /// Encrypt string with SHA1
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(string content, bool isToLower = false, Encoding? encoding = null)
        => Encrypt(EncryptType.Sha1, content, isToLower, encoding);
}
