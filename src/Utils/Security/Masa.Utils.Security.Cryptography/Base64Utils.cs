// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

/// <summary>
/// Base64 encryption and decryption
/// </summary>
public class Base64Utils : EncryptBase
{
    /// <summary>
    /// Base64 encryption
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted data</returns>
    public static string Encrypt(string content, Encoding? encoding = null)
    {
        byte[] buffers = GetSafeEncoding(encoding).GetBytes(content);
        return buffers.ToBase64String();
    }

    /// <summary>
    /// Base64 decryption
    /// </summary>
    /// <param name="content">String to decrypt</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>decrypted data</returns>
    public static string Decrypt(string content, Encoding? encoding = null)
    {
        byte[] buffers = Convert.FromBase64String(content);
        return buffers.ConvertToString(GetSafeEncoding(encoding));
    }
}
