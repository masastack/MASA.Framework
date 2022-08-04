// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

public class AesUtils : EncryptBase
{
    private static readonly byte[] DefaultIv =
    {
        0x41,
        0x72,
        0x65,
        0x79,
        0x6F,
        0x75,
        0x6D,
        0x79,
        0x53,
        0x6E,
        0x6F,
        0x77,
        0x6D,
        0x61,
        0x6E,
        0x3F
    };

    /// <summary>
    /// Generate a key that complies with AES encryption rules
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateKey(int length)
    {
        var crypto = Aes.Create();
        crypto.KeySize = length;
        crypto.BlockSize = 128;
        crypto.GenerateKey();
        return Convert.ToBase64String(crypto.Key);
    }

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged encryption (RijndaelManaged (AES) algorithm is a block encryption algorithm)
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Encrypt(content, GlobalConfigurationUtils.DefaultEncryKey, FillType.Right, fillCharacter, encoding);

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged encryption (RijndaelManaged (AES) algorithm is a block encryption algorithm)
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        string key,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Encrypt(content, key, DefaultIv, fillType, fillCharacter, encoding);

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged encryption (RijndaelManaged (AES) algorithm is a block encryption algorithm)
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length key or complement by fillType to calculate an 16-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        string key,
        string iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var ivBuffer = GetSafeEncoding(encoding).GetBytes(GetSpecifiedLengthString(
            iv,
            16,
            () => throw new ArgumentException(nameof(key),
                $"Please enter a 16-bit iv or allow {nameof(fillType)} to Left or Right"),
            fillType,
            fillCharacter));

        return Encrypt(content,
            key,
            ivBuffer,
            fillType,
            fillCharacter,
            encoding);
    }

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged encryption (RijndaelManaged (AES) algorithm is a block encryption algorithm)
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length or complement by fillType to calculate an 16-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        string key,
        byte[] iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        key = GetSpecifiedLengthString(
            key,
            32,
            () => throw new ArgumentException(nameof(key),
                $"Please enter a 32-bit AES key or allow {nameof(fillType)} to Left or Right"),
            fillType,
            fillCharacter);
        using var aes = Aes.Create();
        aes.Key = currentEncoding.GetBytes(key);
        aes.IV = iv;
        using ICryptoTransform cryptoTransform = aes.CreateEncryptor();
        byte[] buffers = currentEncoding.GetBytes(content);
        byte[] encryptedData = cryptoTransform.TransformFinalBlock(buffers, 0, buffers.Length);
        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged decrypts the string
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>If the decryption succeeds, the decrypted string will be returned, and if it fails, the source string will be returned.</returns>
    public static string Decrypt(
        string content,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Decrypt(content, GlobalConfigurationUtils.DefaultEncryKey, FillType.Right, fillCharacter, encoding);

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged decrypts the string
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>Decryption success returns the decrypted string, failure returns empty</returns>
    public static string Decrypt(
        string content,
        string key,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Decrypt(content, key, DefaultIv, fillType, fillCharacter, encoding);

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged decrypts the string
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length or complement by fillType to calculate an 16-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>Decryption success returns the decrypted string, failure returns empty</returns>
    public static string Decrypt(
        string content,
        string key,
        string iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var ivBuffer = GetSafeEncoding(encoding).GetBytes(GetSpecifiedLengthString(
            iv,
            16,
            () => throw new ArgumentException(nameof(key),
                $"Please enter a 16-bit iv or allow {nameof(fillType)} to Left or Right"),
            fillType,
            fillCharacter));

        return Decrypt(content,
            key,
            ivBuffer,
            fillType,
            fillCharacter,
            encoding);
    }

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged decrypts the string
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length. 16-bit length key or complement by fillType to calculate an 16-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>Decryption success returns the decrypted string, failure returns empty</returns>
    public static string Decrypt(
        string content,
        string key,
        byte[] iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        key = GetSpecifiedLengthString(
            key,
            32,
            () => throw new ArgumentException(nameof(key),
                $"Please enter a 32-bit AES key or allow {nameof(fillType)} to Left or Right"),
            fillType,
            fillCharacter);
        using var aes = Aes.Create();
        aes.Key = currentEncoding.GetBytes(key);
        aes.IV = iv;
        using ICryptoTransform rijndaelDecrypt = aes.CreateDecryptor();
        byte[] buffers = Convert.FromBase64String(content);
        byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(buffers, 0, buffers.Length);
        return currentEncoding.GetString(decryptedData);
    }

    /// <summary>
    /// encrypted file stream
    /// </summary>
    /// <param name="fileStream">File streams that require encryption</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted stream result</returns>
    public static CryptoStream Encrypt(
        FileStream fileStream,
        string key,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => Encrypt(fileStream, key, key, fillType, fillCharacter, encoding);

    /// <summary>
    /// encrypted file stream
    /// </summary>
    /// <param name="fileStream">File streams that require encryption</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length. 16-bit length key or complement by fillType to calculate an 16-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted stream result</returns>
    public static CryptoStream Encrypt(
        FileStream fileStream,
        string key,
        string iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);

        var ivBuffer = currentEncoding.GetBytes(GetSpecifiedLengthString(iv,
            16,
            () => throw new ArgumentException(nameof(iv),
                $"Please enter a 16-bit iv or allow {nameof(fillType)} to Left or Right"),
            fillType,
            fillCharacter));

        return Encrypt(fileStream, key, ivBuffer, fillType, fillCharacter, encoding);
    }

    /// <summary>
    /// encrypted file stream
    /// </summary>
    /// <param name="fileStream">File streams that require encryption</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted stream result</returns>
    public static CryptoStream Encrypt(
        FileStream fileStream,
        string key,
        byte[] iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        if (iv.Length != 16)
        {
            throw new Exception($"The {nameof(iv)} length is invalid. The {nameof(iv)} iv length needs 16 bits！");
        }

        var currentEncoding = GetSafeEncoding(encoding);
        key = GetSpecifiedLengthString(key,
            32,
            () => throw new ArgumentException(nameof(key),
                $"Please enter a 32-bit AES key or allow {nameof(fillType)} to Left or Right"),
            fillType,
            fillCharacter);

        using var aes = Aes.Create();
        aes.Key = currentEncoding.GetBytes(key);
        aes.IV = iv;
        using var cryptoTransform = aes.CreateEncryptor();
        return new CryptoStream(fileStream, cryptoTransform, CryptoStreamMode.Write);
    }

    /// <summary>
    /// Decrypt the file stream
    /// </summary>
    /// <param name="fileStream">file stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>Decrypt the stream result</returns>
    public static CryptoStream Decrypt(
        FileStream fileStream,
        string key,
        string iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);

        var ivBuffer = currentEncoding.GetBytes(GetSpecifiedLengthString(iv,
            16,
            () => throw new ArgumentException(nameof(iv),
                $"Please enter a 16-bit iv or allow {nameof(fillType)} to Left or Right"),
            fillType,
            fillCharacter));

        return Decrypt(fileStream, key, ivBuffer, fillType, fillCharacter, encoding);
    }

    /// <summary>
    /// Decrypt the file stream
    /// </summary>
    /// <param name="fileStream">file stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>Decrypt the stream result</returns>
    public static CryptoStream Decrypt(
        FileStream fileStream,
        string key,
        byte[] iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        if (iv.Length != 16)
        {
            throw new Exception($"The {nameof(iv)} length is invalid. The {nameof(iv)} iv length needs 16 bits！");
        }

        key = GetSpecifiedLengthString(key,
            32,
            () => throw new ArgumentException(nameof(key),
                $"Please enter a 32-bit AES key or allow {nameof(fillType)} to Left or Right"),
            fillType,
            fillCharacter);

        var currentEncoding = GetSafeEncoding(encoding);
        using var aes = Aes.Create();
        aes.Key = currentEncoding.GetBytes(key);
        aes.IV = iv;
        using var cryptoTransform = aes.CreateDecryptor();
        return new CryptoStream(fileStream, cryptoTransform, CryptoStreamMode.Read);
    }

    /// <summary>
    /// Encrypt the specified stream with AES and output a file
    /// </summary>
    /// <param name="fileStream">file stream to be encrypted</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        FileStream fileStream,
        string outputPath,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptFile(fileStream,
            GlobalConfigurationUtils.DefaultEncryKey,
            outputPath,
            FillType.Right,
            fillCharacter,
            encoding);

    /// <summary>
    /// Encrypt the specified stream with AES and output a file
    /// </summary>
    /// <param name="fileStream">file stream to be encrypted</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        FileStream fileStream,
        string key,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptFile(fileStream, key, key, outputPath, fillType, fillCharacter, encoding);

    /// <summary>
    /// Encrypt the specified stream with AES and output a file
    /// </summary>
    /// <param name="fileStream">file stream to be encrypted</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length or complement by fillType to calculate an 16-bit string</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        FileStream fileStream,
        string key,
        string iv,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        using var fileStreamOut = new FileStream(outputPath, FileMode.Create);
        using var cryptoStream = Encrypt(fileStream, key, iv, fillType, fillCharacter, encoding);
        byte[] buffers = new byte[1024];
        while (true)
        {
            var count = cryptoStream.Read(buffers, 0, buffers.Length);
            fileStreamOut.Write(buffers, 0, count);
            if (count < buffers.Length)
            {
                break;
            }
        }
    }

    /// <summary>
    /// AES decrypt the specified file stream and output the file
    /// </summary>
    /// <param name="fileStream">file stream to be decrypted</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        FileStream fileStream,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => DecryptFile(fileStream, GlobalConfigurationUtils.DefaultEncryKey, outputPath, fillType, fillCharacter, encoding);

    /// <summary>
    /// AES decrypt the specified file stream and output the file
    /// </summary>
    /// <param name="fileStream">file stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        FileStream fileStream,
        string key,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => DecryptFile(fileStream, key, key, outputPath, fillType, fillCharacter, encoding);

    /// <summary>
    /// AES decrypt the specified file stream and output the file
    /// </summary>
    /// <param name="fileStream">file stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length or complement by fillType to calculate an 16-bit string</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        FileStream fileStream,
        string key,
        string iv,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        using FileStream fileStreamOut = new(outputPath, FileMode.Create);
        using CryptoStream cryptoStream = Decrypt(fileStream, key, iv, fillType, fillCharacter, encoding);
        byte[] buffers = new byte[1024];
        while (true)
        {
            var count = cryptoStream.Read(buffers, 0, buffers.Length);
            fileStreamOut.Write(buffers, 0, count);
            if (count < buffers.Length)
            {
                break;
            }
        }
    }
}
