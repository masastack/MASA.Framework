// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

/// <summary>
/// DES symmetric encryption and decryption
/// </summary>
public class DesUtils : EncryptBase
{
    /// <summary>
    /// Default encryption key
    /// </summary>
    private static readonly string DefaultEncryptKey = MD5Utils.EncryptRepeat(GlobalConfigurationUtils.DefaultEncryKey, 2);

    /// <summary>
    /// 使用默认加密
    /// </summary>
    /// <param name="content">被加密的字符串</param>
    /// <param name="desEncryType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        DESEncryType desEncryType = DESEncryType.Improved,
        bool isToLower = true,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Encrypt(content, DefaultEncryptKey, desEncryType, isToLower, FillType.Right, fillCharacter, encoding);

    /// <summary>
    /// Des encrypted string
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="desEncryType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        string key,
        DESEncryType desEncryType = DESEncryType.Improved,
        bool isToLower = true,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Encrypt(content, key, key, desEncryType, isToLower, fillType, fillCharacter, encoding);

    /// <summary>
    /// Des encrypted string
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="iv">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="desEncryType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="isToLower">Whether to convert the encrypted string to lowercase</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        string key,
        string iv,
        DESEncryType desEncryType = DESEncryType.Improved,
        bool isToLower = true,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
#pragma warning disable S2278
        var des = DES.Create();
        des.Key = currentEncoding.GetBytes(
            GetSpecifiedLengthString(key,
                8,
                () => throw new ArgumentException($"Please enter a 8-bit DES key or allow {nameof(fillType)} to Left or Right"),
                fillType,
                fillCharacter));
        des.IV = currentEncoding.GetBytes(
            GetSpecifiedLengthString(iv,
                8,
                () => throw new ArgumentException($"Please enter a 8-bit DES iv or allow {nameof(fillType)} to Left or Right"),
                fillType,
                fillCharacter));

        using MemoryStream memoryStream = new MemoryStream();
        byte[] buffer = currentEncoding.GetBytes(content);
        using CryptoStream cs = new CryptoStream(memoryStream, des.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(buffer, 0, buffer.Length);
        cs.FlushFinalBlock();
        if (desEncryType == DESEncryType.Normal)
            return Convert.ToBase64String(memoryStream.ToArray());

        StringBuilder stringBuilder = new();
        foreach (byte b in memoryStream.ToArray())
        {
            stringBuilder.AppendFormat(isToLower ? $"{b:x2}" : $"{b:X2}");
        }

        return stringBuilder.ToString();
#pragma warning restore S2278
    }

    /// <summary>
    /// DES decryption with default key
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="desEncryType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>decrypted result</returns>
    public static string Decrypt(string content,
        DESEncryType desEncryType = DESEncryType.Improved,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Decrypt(content, DefaultEncryptKey, desEncryType, FillType.Right, fillCharacter, encoding);

    /// <summary>
    /// DES decryption
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="key">8-bit length key</param>
    /// <param name="desEncryType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>decrypted result</returns>
    public static string Decrypt(
        string content,
        string key,
        DESEncryType desEncryType = DESEncryType.Improved,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Decrypt(content, key, key, desEncryType, fillType, fillCharacter, encoding);

    /// <summary>
    /// DES decryption
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="iv">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="desEncryType">Des encryption method, default: improved (easy to transmit)</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>decrypted result</returns>
    public static string Decrypt(
        string content,
        string key,
        string iv,
        DESEncryType desEncryType = DESEncryType.Improved,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        using var memoryStream = new MemoryStream();
#pragma warning disable S2278
        using var des = DES.Create();
        var currentEncoding = GetSafeEncoding(encoding);
        des.Key = currentEncoding.GetBytes(
            GetSpecifiedLengthString(key,
                8,
                () => throw new ArgumentException($"Please enter a 8-bit DES key or allow {nameof(fillType)} to Left or Right"),
                fillType,
                fillCharacter));
        des.IV = currentEncoding.GetBytes(
            GetSpecifiedLengthString(iv,
                8,
                () => throw new ArgumentException($"Please enter a 8-bit DES iv or allow {nameof(fillType)} to Left or Right"),
                fillType,
                fillCharacter));

        using (MemoryStream ms = new MemoryStream())
        {
            byte[] buffers = desEncryType == DESEncryType.Improved ? new byte[content.Length / 2] : Convert.FromBase64String(content);
            if (desEncryType == DESEncryType.Improved)
            {
                for (int x = 0; x < content.Length / 2; x++)
                {
                    int i = Convert.ToInt32(content.Substring(x * 2, 2), 16);
                    buffers[x] = (byte)i;
                }
            }

            using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(buffers, 0, buffers.Length);
                cs.FlushFinalBlock();
            }

            return currentEncoding.GetString(ms.ToArray());
        }
#pragma warning restore S2278
    }

    /// <summary>
    /// DES encrypts the file stream and outputs the encrypted file
    /// </summary>
    /// <param name="fileStream">file input stream</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        FileStream fileStream,
        string outFilePath,
        string key,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        byte[] iv =
        {
            0x12,
            0x34,
            0x56,
            0x78,
            0x90,
            0xAB,
            0xCD,
            0xEF
        };
        EncryptFile(fileStream, outFilePath, key, iv, fillType, fillCharacter, encoding);
    }

    /// <summary>
    /// DES encrypts the file stream and outputs the encrypted file
    /// </summary>
    /// <param name="fileStream">file input stream</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="key">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="iv">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        FileStream fileStream,
        string outFilePath,
        string key,
        string iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        var ivBuffer = currentEncoding.GetBytes(
            GetSpecifiedLengthString(iv,
                8,
                () => throw new ArgumentException($"Please enter a 8-bit DES iv or allow {nameof(fillType)} to Left or Right"),
                fillType,
                fillCharacter));
        EncryptFile(fileStream, outFilePath, key, ivBuffer, fillType, fillCharacter, encoding);
    }

    /// <summary>
    /// DES encrypts the file stream and outputs the encrypted file
    /// </summary>
    /// <param name="fileStream">file input stream</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="key">8-bit length key</param>
    /// <param name="iv">8-bit length key</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        FileStream fileStream,
        string outFilePath,
        string key,
        byte[] iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        if (iv.Length != 8)
        {
            throw new Exception($"The {nameof(iv)} length is invalid. The {nameof(iv)} iv length needs 8 bits！");
        }

        var currentEncoding = GetSafeEncoding(encoding);
        using var fileStreamOut = new FileStream(outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        fileStreamOut.SetLength(0);
        byte[] buffers = new byte[100];
        long readLength = 0;

#pragma warning disable S2278
        using var des = DES.Create();
        des.Key = currentEncoding.GetBytes(
            GetSpecifiedLengthString(key,
                8,
                () => throw new ArgumentException($"Please enter a 8-bit DES key or allow {nameof(fillType)} to Left or Right"),
                fillType,
                fillCharacter));
        des.IV = iv;

        using var cryptoStream = new CryptoStream(fileStreamOut, des.CreateEncryptor(),
            CryptoStreamMode.Write);
        while (readLength < fileStream.Length)
        {
            var length = fileStream.Read(buffers, 0, 100);
            cryptoStream.Write(buffers, 0, length);
            readLength += length;
        }
#pragma warning restore S2278
    }

    /// <summary>
    /// DES decrypts the file stream and outputs the source file
    /// </summary>
    /// <param name="fileStream">input file stream to be decrypted</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="key">decryption key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        FileStream fileStream,
        string outFilePath,
        string key,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        byte[] iv =
        {
            0x12,
            0x34,
            0x56,
            0x78,
            0x90,
            0xAB,
            0xCD,
            0xEF
        };
        DecryptFile(fileStream, outFilePath, key, iv, fillType, fillCharacter, encoding);
    }

    /// <summary>
    /// DES decrypts the file stream and outputs the source file
    /// </summary>
    /// <param name="fileStream">input file stream to be decrypted</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="key">decryption key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="iv">8-bit length key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        FileStream fileStream,
        string outFilePath,
        string key,
        string iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);

        var ivBuffer = currentEncoding.GetBytes(
            GetSpecifiedLengthString(iv,
                8,
                () => throw new ArgumentException($"Please enter a 8-bit DES iv or allow {nameof(fillType)} to Left or Right"),
                fillType,
                fillCharacter));

        DecryptFile(fileStream, outFilePath, key, ivBuffer, fillType, fillCharacter, currentEncoding);
    }

    /// <summary>
    /// DES decrypts the file stream and outputs the source file
    /// </summary>
    /// <param name="fileStream">input file stream to be decrypted</param>
    /// <param name="outFilePath">file output path</param>
    /// <param name="key">decryption key or complement by fillType to calculate an 8-bit string</param>
    /// <param name="iv"></param>
    /// <param name="fillType">Do you supplement key and iv? default: no fill(Only supports 8-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        FileStream fileStream,
        string outFilePath,
        string key,
        byte[] iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        if (iv.Length != 8)
        {
            throw new Exception($"The {nameof(iv)} length is invalid. The {nameof(iv)} iv length needs 8 bits！");
        }

        var currentEncoding = GetSafeEncoding(encoding);
        using var fileStreamOut = new FileStream(outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        fileStreamOut.SetLength(0);
        byte[] buffers = new byte[100];
        long readLength = 0;
#pragma warning disable S2278
        using var des = DES.Create();
        des.Key = currentEncoding.GetBytes(
            GetSpecifiedLengthString(key,
                8,
                () => throw new ArgumentException($"Please enter a 8-bit DES key or allow {nameof(fillType)} to Left or Right"),
                fillType,
                fillCharacter));
        des.IV = iv;
        using var cryptoStream = new CryptoStream(fileStreamOut, des.CreateDecryptor(),
            CryptoStreamMode.Write);
        while (readLength < fileStream.Length)
        {
            var length = fileStream.Read(buffers, 0, 100);
            cryptoStream.Write(buffers, 0, length);
            readLength += length;
        }
#pragma warning restore S2278
    }
}
