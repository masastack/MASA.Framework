// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

/// <summary>
/// DES symmetric encryption and decryption
/// </summary>
#pragma warning disable S2342
// ReSharper disable once InconsistentNaming
public class DESUtils : EncryptBase
{
    /// <summary>
    /// Default encryption key
    /// </summary>
    private static readonly string DefaultEncryptKey = GetSpecifiedLengthString(
        MD5Utils.EncryptRepeat(GlobalConfigurationUtils.DefaultEncryptKey, 2), 8,
        () =>
        {

        }, FillType.Right);

    /// <summary>
    /// Default encryption iv
    /// </summary>
    private static readonly string DefaultEncryptIv = DefaultEncryptKey;

#pragma warning disable S5547
#pragma warning disable S107

    /// <summary>
    /// Des encrypted string
    /// </summary>
    /// <param name="content">encrypted string</param>
    /// <param name="desEncryptType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        DESEncryptType desEncryptType = DESEncryptType.Improved,
        bool isToLower = true,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Encrypt(content, DefaultEncryptKey, desEncryptType, isToLower, FillType.Right, fillCharacter, encoding);

    /// <summary>
    /// Des encrypted string
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="desEncryptType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        string key,
        DESEncryptType desEncryptType = DESEncryptType.Improved,
        bool isToLower = true,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Encrypt(content, key, DefaultEncryptIv, desEncryptType, isToLower, fillType, fillCharacter, encoding);

    /// <summary>
    /// Des encrypted string
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="iv">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="desEncryptType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        string key,
        string iv,
        DESEncryptType desEncryptType = DESEncryptType.Improved,
        bool isToLower = true,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        using MemoryStream memoryStream = new MemoryStream();
        byte[] buffer = currentEncoding.GetBytes(content);
        var des = DES.Create();
        using CryptoStream cs = new CryptoStream(memoryStream,
            des.CreateEncryptor(
                GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
                GetKeyBuffer(iv, currentEncoding, fillType, fillCharacter)),
            CryptoStreamMode.Write);
        cs.Write(buffer, 0, buffer.Length);
        cs.FlushFinalBlock();
        if (desEncryptType == DESEncryptType.Normal)
            return memoryStream.ToArray().ToBase64String();

        StringBuilder stringBuilder = new();
        foreach (byte b in memoryStream.ToArray())
        {
            stringBuilder.AppendFormat(isToLower ? $"{b:x2}" : $"{b:X2}");
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// DES decryption with default key
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="desEncryptType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>decrypted result</returns>
    public static string Decrypt(string content,
        DESEncryptType desEncryptType = DESEncryptType.Improved,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Decrypt(content, DefaultEncryptKey, desEncryptType, FillType.Right, fillCharacter, encoding);

    /// <summary>
    /// DES decryption
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="key">8-bit length key</param>
    /// <param name="desEncryptType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>decrypted result</returns>
    public static string Decrypt(
        string content,
        string key,
        DESEncryptType desEncryptType = DESEncryptType.Improved,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Decrypt(content, key, DefaultEncryptIv, desEncryptType, fillType, fillCharacter, encoding);

    /// <summary>
    /// DES decryption
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="iv">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="desEncryptType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>decrypted result</returns>
    public static string Decrypt(
        string content,
        string key,
        string iv,
        DESEncryptType desEncryptType = DESEncryptType.Improved,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        using var memoryStream = new MemoryStream();
        var currentEncoding = GetSafeEncoding(encoding);

        using MemoryStream ms = new MemoryStream();
        byte[] buffers = desEncryptType == DESEncryptType.Improved ? new byte[content.Length / 2] : content.FromBase64String();
        if (desEncryptType == DESEncryptType.Improved)
        {
            for (int x = 0; x < content.Length / 2; x++)
            {
                int i = Convert.ToInt32(content.Substring(x * 2, 2), 16);
                buffers[x] = (byte)i;
            }
        }

        using var des = DES.Create();
        using (CryptoStream cs = new CryptoStream(ms,
                   des.CreateDecryptor(GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
                       GetIvBuffer(iv, currentEncoding, fillType, fillCharacter)), CryptoStreamMode.Write))
        {
            cs.Write(buffers, 0, buffers.Length);
            cs.FlushFinalBlock();
        }

        return currentEncoding.GetString(ms.ToArray());

    }

    /// <summary>
    /// DES encrypts the file stream and outputs the encrypted file
    /// </summary>
    /// <param name="fileStream">file input stream</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        FileStream fileStream,
        string key,
        string outFilePath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptOrDecryptFile(fileStream, key, outFilePath, true, fillType, fillCharacter, encoding);

    /// <summary>
    /// DES encrypts the file stream and outputs the encrypted file
    /// </summary>
    /// <param name="fileStream">file input stream</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="iv">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        FileStream fileStream,
        string key,
        string iv,
        string outFilePath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptOrDecryptFile(fileStream, outFilePath, (key, iv), true, fillType, fillCharacter, encoding);

    /// <summary>
    /// DES encrypts the file stream and outputs the encrypted file
    /// </summary>
    /// <param name="fileStream">file input stream</param>
    /// <param name="key">8-bit length key</param>
    /// <param name="ivBuffer">8-bit length key</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        FileStream fileStream,
        string key,
        byte[] ivBuffer,
        string outFilePath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptOrDecryptFile(fileStream, outFilePath, (key, ivBuffer), true, fillType, fillCharacter, encoding);

    /// <summary>
    /// DES decrypts the file stream and outputs the source file
    /// </summary>
    /// <param name="fileStream">input file stream to be decrypted</param>
    /// <param name="key">decryption key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        FileStream fileStream,
        string key,
        string outFilePath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptOrDecryptFile(fileStream, key, outFilePath, false, fillType, fillCharacter, encoding);

    /// <summary>
    /// DES decrypts the file stream and outputs the source file
    /// </summary>
    /// <param name="fileStream">input file stream to be decrypted</param>
    /// <param name="key">decryption key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="iv">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        FileStream fileStream,
        string key,
        string iv,
        string outFilePath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptOrDecryptFile(fileStream, outFilePath, (key, iv), false, fillType, fillCharacter, encoding);

    /// <summary>
    /// DES decrypts the file stream and outputs the source file
    /// </summary>
    /// <param name="fileStream">input file stream to be decrypted</param>
    /// <param name="key">decryption key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="ivBuffer"></param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        FileStream fileStream,
        string key,
        byte[] ivBuffer,
        string outFilePath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptOrDecryptFile(fileStream, outFilePath, (key, ivBuffer), false, fillType, fillCharacter, encoding);

    private static void EncryptOrDecryptFile(
        FileStream fileStream,
        string key,
        string outFilePath,
        bool isEncrypt,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptOrDecryptFile(fileStream, outFilePath, (key, DefaultEncryptKey), isEncrypt, fillType, fillCharacter, encoding);

    private static void EncryptOrDecryptFile(
        FileStream fileStream,
        string outFilePath,
        (string Key, string IV) keyIv,
        bool isEncrypt,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        var ivBuffer = GetIvBuffer(keyIv.IV, currentEncoding, fillType, fillCharacter);
        EncryptOrDecryptFile(fileStream, outFilePath, (keyIv.Key, ivBuffer), isEncrypt, fillType, fillCharacter, encoding);
    }

    private static void EncryptOrDecryptFile(
        FileStream fileStream,
        string outFilePath,
        (string Key, byte[] IV) keyIv,
        bool isEncrypt,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        if (keyIv.IV.Length != 8)
        {
            throw new ArgumentException($"The iv length is invalid. The iv length needs 8 bits！");
        }

        var currentEncoding = GetSafeEncoding(encoding);
        using var fileStreamOut = new FileStream(outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        fileStreamOut.SetLength(0);
        byte[] buffers = new byte[100];
        long readLength = 0;

        using var des = DES.Create();
        des.Key = GetKeyBuffer(keyIv.Key, currentEncoding, fillType, fillCharacter);
        des.IV = keyIv.IV;

        using var cryptoStream = new CryptoStream(fileStreamOut,
            isEncrypt ? des.CreateEncryptor() : des.CreateDecryptor(),
            CryptoStreamMode.Write);
        while (readLength < fileStream.Length)
        {
            var length = fileStream.Read(buffers, 0, 100);
            cryptoStream.Write(buffers, 0, length);
            readLength += length;
        }
    }

    private static byte[] GetKeyBuffer(string key,
        Encoding encoding,
        FillType fillType,
        char fillCharacter)
        => GetBytes(
            key,
            encoding,
            fillType,
            fillCharacter,
            nameof(key),
            $"Please enter a 8-bit DES key or allow {nameof(fillType)} to Left or Right");

    private static byte[] GetIvBuffer(string iv,
        Encoding encoding,
        FillType fillType,
        char fillCharacter)
        => GetBytes(
            iv,
            encoding,
            fillType,
            fillCharacter,
            nameof(iv),
            $"Please enter a 8-bit DES iv or allow {nameof(fillType)} to Left or Right");

    private static byte[] GetBytes(string str,
        Encoding encoding,
        FillType fillType,
        char fillCharacter,
        string paramName,
        string message)
    {
        return GetBytes(
            str,
            encoding,
            fillType,
            fillCharacter,
            8,
            () => throw new ArgumentException(message, paramName));
    }

#pragma warning restore S107
#pragma warning restore S2278
}
#pragma warning restore S2342
