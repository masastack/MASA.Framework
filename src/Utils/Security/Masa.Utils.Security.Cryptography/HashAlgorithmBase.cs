// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

public class HashAlgorithmBase : EncryptBase
{
    /// <summary>
    /// encryption
    /// </summary>
    /// <param name="encryptType"></param>
    /// <param name="content">String to be encrypted</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string Encrypt(EncryptType encryptType, string content, bool isToLower = true, Encoding? encoding = null)
    {
        using (var hashAlgorithm = HashAlgorithm.Create(encryptType.ToString()))
        {
            if (hashAlgorithm == null)
                throw new NotSupportedException("Unsupported encryptType");

            byte[] buffer = GetSafeEncoding(encoding).GetBytes(content);
            buffer = hashAlgorithm.ComputeHash(buffer);
            return Encrypt(encryptType, buffer, hashAlgorithm, isToLower);
        }
    }

    protected static string Encrypt(EncryptType encryptType, byte[] buffer, HashAlgorithm? hashAlgorithm = null, bool isToLower = true)
    {
        using (hashAlgorithm ??= HashAlgorithm.Create(encryptType.ToString()))
        {
            if (hashAlgorithm == null)
                throw new NotSupportedException("Unsupported encryptType");

            hashAlgorithm.Clear();

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in buffer)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }

            var result = BitConverter.ToString(buffer).Replace("-", "");
            return isToLower ? result.ToLower() : result;
        }
    }
}
