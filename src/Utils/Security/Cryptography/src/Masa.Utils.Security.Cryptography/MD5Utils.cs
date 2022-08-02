// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

/// <summary>
/// MD5加密算法
/// </summary>
public class MD5Utils : HashAlgorithmBase
{
    /// <summary>
    /// MD5 encryption of string
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns></returns>
    private static string Encrypt(
        string content,
        bool isToLower = true,
        Encoding? encoding = null)
        => Encrypt(content, string.Empty, isToLower, encoding);

    /// <summary>
    /// MD5 multiple encryption
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="encryptTimes">Encryption times,default: 1</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string EncryptRepeat(
        string content,
        int encryptTimes = 1,
        bool isToLower = true,
        Encoding? encoding = null)
        => EncryptRepeat(content, string.Empty, encryptTimes, false, isToLower, encoding);

    /// <summary>
    /// MD5 salt-encrypted string
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="salt"></param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        string salt,
        bool isToLower = true,
        Encoding? encoding = null)
        => Encrypt(EncryptType.Md5, content + salt, isToLower, encoding);

    /// <summary>
    /// MD5 multiple encryption
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="salt"></param>
    /// <param name="encryptTimes"></param>
    /// <param name="isNeedSalt">When the number of executions is greater than 1, is it necessary to add salt and then encrypt after the second time? default: false (Salt encryption only for the first time)</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string EncryptRepeat(
        string content,
        string salt,
        int encryptTimes,
        bool isNeedSalt = false,
        bool isToLower = true,
        Encoding? encoding = null)
    {
        if (encryptTimes < 1)
            throw new ArgumentException($"{nameof(encryptTimes)} must be greater than or equal to 1", nameof(encryptTimes));

        int times = 1;
        string result = Encrypt(content + salt, isToLower, encoding);
        while (times < encryptTimes)
        {
            result = isNeedSalt ? result + salt : result;
            result = Encrypt(result, isToLower, encoding);
            times++;
        }

        return result;
    }

    /// <summary>
    /// Get the MD5 value of the file
    /// </summary>
    /// <param name="fileName">absolute path to the file</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <returns>encrypted result</returns>
    public static string EncryptFile(string fileName, bool isToLower = true)
    {
        using var fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
        return EncryptStream(fileStream, isToLower);
    }

    /// <summary>
    /// Get the MD5 value of the data stream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <returns>encrypted result</returns>
    public static string EncryptStream(Stream stream, bool isToLower = true)
    {
        stream.Position = 0;
        byte[] buffers = new byte[stream.Length];
        stream.Read(buffers, 0, buffers.Length);
        stream.Seek(0, SeekOrigin.Begin);
        using var md5 = MD5.Create();
        byte[] bytes = md5.ComputeHash(buffers);
        var encryptedContent = Encrypt(EncryptType.Md5, bytes, null, isToLower);
        stream.Position = 0;
        return encryptedContent;
    }
}
