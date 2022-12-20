// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

public class AesUtils : EncryptBase
{
    private static readonly string DefaultEncryptKey = GetSpecifiedLengthString(GlobalConfigurationUtils.DefaultEncryptKey, 32, () =>
    {
    }, FillType.Right, ' ');

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
        return crypto.Key.ToBase64String();
    }

    #region Encrypt

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
        => Encrypt(content, DefaultEncryptKey, FillType.Right, fillCharacter, encoding);

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
        var currentEncoding = GetSafeEncoding(encoding);
        var byteBuffer = EncryptToBytes(
            currentEncoding.GetBytes(content),
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            GetIvBuffer(iv, currentEncoding, fillType, fillCharacter)
        );
        return byteBuffer.ToBase64String();
    }

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged encryption (RijndaelManaged (AES) algorithm is a block encryption algorithm)
    /// </summary>
    /// <param name="content">String to be encrypted</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="ivBuffer">16-bit length or complement by fillType to calculate an 16-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>encrypted result</returns>
    public static string Encrypt(
        string content,
        string key,
        byte[] ivBuffer,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        var byteBuffer = EncryptToBytes(
            currentEncoding.GetBytes(content),
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            ivBuffer);
        return byteBuffer.ToBase64String();
    }

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged encryption (RijndaelManaged (AES) algorithm is a block encryption algorithm)
    /// </summary>
    /// <param name="dataBuffers">data array to be encrypted</param>
    /// <param name="keyBuffer">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="ivBuffer">16-bit length or complement by fillType to calculate an 16-bit string</param>
    /// <returns>encrypted result</returns>
    public static byte[] EncryptToBytes(
        byte[] dataBuffers,
        byte[] keyBuffer,
        byte[] ivBuffer)
        => EncryptOrDecryptToBytes(dataBuffers, keyBuffer, ivBuffer, true);

    /// <summary>
    /// encrypted stream
    /// </summary>
    /// <param name="stream">streams that require encryption</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>Returns the encrypted byte array</returns>
    public static byte[] EncryptToBytes(
        Stream stream,
        string key,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptToBytes(stream, key, DefaultIv, fillType, fillCharacter, encoding);

    /// <summary>
    /// encrypted stream
    /// </summary>
    /// <param name="stream">streams that require encryption</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length. 16-bit length key or complement by fillType to calculate an 16-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>Returns the encrypted byte array</returns>
    public static byte[] EncryptToBytes(
        Stream stream,
        string key,
        string iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);

        return EncryptToBytes(stream,
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            GetIvBuffer(iv, currentEncoding, fillType, fillCharacter));
    }

    /// <summary>
    /// encrypted stream
    /// </summary>
    /// <param name="stream">streams that require encryption</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="ivBuffer">16-bit length</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <returns>Returns the encrypted byte array</returns>
    public static byte[] EncryptToBytes(
        Stream stream,
        string key,
        byte[] ivBuffer,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        return EncryptToBytes(
            stream,
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            ivBuffer);
    }

    /// <summary>
    /// encrypted stream
    /// </summary>
    /// <param name="stream">streams that require encryption</param>
    /// <param name="keyBuffer">Encryption key, 32-bit length</param>
    /// <param name="ivBuffer">16-bit length</param>
    /// <returns>Returns the encrypted byte array</returns>
    public static byte[] EncryptToBytes(
        Stream stream,
        byte[] keyBuffer,
        byte[] ivBuffer)
        => EncryptOrDecryptToBytes(stream, keyBuffer, ivBuffer, true);

    /// <summary>
    /// Encrypt the specified stream with AES and output a file
    /// </summary>
    /// <param name="stream">stream to be encrypted</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        Stream stream,
        string outputPath,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptFile(stream,
            DefaultEncryptKey,
            outputPath,
            FillType.Right,
            fillCharacter,
            encoding);

    /// <summary>
    /// Encrypt the specified stream with AES and output a file
    /// </summary>
    /// <param name="stream">stream to be encrypted</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        Stream stream,
        string key,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => EncryptFile(stream, key, DefaultIv, outputPath, fillType, fillCharacter, encoding);

    /// <summary>
    /// Encrypt the specified stream with AES and output a file
    /// </summary>
    /// <param name="stream">stream to be encrypted</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="ivBuffer">16-bit length</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        Stream stream,
        string key,
        byte[] ivBuffer,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        EncryptOrDecryptFile(
            stream,
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            ivBuffer,
            true,
            outputPath);
    }

    /// <summary>
    /// Encrypt the specified stream with AES and output a file
    /// </summary>
    /// <param name="stream">stream to be encrypted</param>
    /// <param name="key">Encryption key, must have half-width characters. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length or complement by fillType to calculate an 16-bit string</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void EncryptFile(
        Stream stream,
        string key,
        string iv,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        EncryptOrDecryptFile(
            stream,
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            GetIvBuffer(iv, currentEncoding, fillType, fillCharacter),
            true,
            outputPath);
    }

    #endregion

    #region Decrypt

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
        => Decrypt(content, DefaultEncryptKey, FillType.Right, fillCharacter, encoding);

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
        var currentEncoding = GetSafeEncoding(encoding);
        var decryptBuffer = DecryptToBytes(
            content.FromBase64String(),
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            GetIvBuffer(iv, currentEncoding, fillType, fillCharacter));
        return decryptBuffer.ConvertToString(currentEncoding);
    }

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged decrypts the string
    /// </summary>
    /// <param name="content">String to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="ivBuffer">16-bit length. 16-bit length key or complement by fillType to calculate an 16-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>Decryption success returns the decrypted string, failure returns empty</returns>
    public static string Decrypt(
        string content,
        string key,
        byte[] ivBuffer,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        var decryptBuffer = DecryptToBytes(
            content.FromBase64String(),
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            ivBuffer
        );
        return decryptBuffer.ConvertToString(currentEncoding);
    }

    /// <summary>
    /// Symmetric encryption algorithm AES RijndaelManaged decrypts
    /// </summary>
    /// <param name="dataBuffers">data array to be decrypted</param>
    /// <param name="keyBuffer">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="ivBuffer">16-bit length. 16-bit length key or complement by fillType to calculate an 16-bit string</param>
    /// <returns></returns>
    public static byte[] DecryptToBytes(
        byte[] dataBuffers,
        byte[] keyBuffer,
        byte[] ivBuffer)
        => EncryptOrDecryptToBytes(
            dataBuffers,
            keyBuffer,
            ivBuffer,
            false);

    /// <summary>
    /// Decrypt the stream
    /// </summary>
    /// <param name="stream">stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>returns the decrypted byte array</returns>
    public static byte[] DecryptToBytes(
        Stream stream,
        string key,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => DecryptToBytes(stream, key, DefaultIv, fillType, fillCharacter, encoding);

    /// <summary>
    /// Decrypt the stream
    /// </summary>
    /// <param name="stream">stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>returns the decrypted byte array</returns>
    public static byte[] DecryptToBytes(
        Stream stream,
        string key,
        string iv,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        return DecryptToBytes(
            stream,
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            GetIvBuffer(iv, currentEncoding, fillType, fillCharacter));
    }

    /// <summary>
    /// Decrypt the stream
    /// </summary>
    /// <param name="stream">stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="ivBuffer">16-bit length</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    /// <returns>returns the decrypted byte array</returns>
    public static byte[] DecryptToBytes(
        Stream stream,
        string key,
        byte[] ivBuffer,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        return DecryptToBytes(stream,
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            ivBuffer);
    }

    /// <summary>
    /// Decrypt the stream
    /// </summary>
    /// <param name="stream">stream to be decrypted</param>
    /// <param name="keyBuffer">Decryption key, 32-bit length</param>
    /// <param name="ivBuffer">16-bit length</param>
    /// <returns>returns the decrypted byte array</returns>
    public static byte[] DecryptToBytes(
        Stream stream,
        byte[] keyBuffer,
        byte[] ivBuffer)
        => EncryptOrDecryptToBytes(stream, keyBuffer, ivBuffer, false);

    /// <summary>
    /// AES decrypt the specified stream and output the file
    /// </summary>
    /// <param name="stream">stream to be decrypted</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        Stream stream,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => DecryptFile(stream, DefaultEncryptKey, outputPath, fillType, fillCharacter, encoding);

    /// <summary>
    /// AES decrypt the specified stream and output the file
    /// </summary>
    /// <param name="stream">stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        Stream stream,
        string key,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
        => DecryptFile(stream, key, DefaultIv, outputPath, fillType, fillCharacter, encoding);

    /// <summary>
    /// AES decrypt the specified stream and output the file
    /// </summary>
    /// <param name="stream">stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="ivBuffer">16-bit length</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        Stream stream,
        string key,
        byte[] ivBuffer,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        EncryptOrDecryptFile(stream, GetKeyBuffer(key, currentEncoding, fillType, fillCharacter), ivBuffer, false, outputPath);
    }

    /// <summary>
    /// AES decrypt the specified stream and output the file
    /// </summary>
    /// <param name="stream">stream to be decrypted</param>
    /// <param name="key">Decryption key, same as encryption key. 32-bit length key or complement by fillType to calculate an 32-bit string</param>
    /// <param name="iv">16-bit length or complement by fillType to calculate an 16-bit string</param>
    /// <param name="outputPath">output file path</param>
    /// <param name="fillType">Whether to complement the key? default: no fill(Only supports 32-bit keys or 16-bit iv)</param>
    /// <param name="fillCharacter">character for complement</param>
    /// <param name="encoding">Encoding format, default UTF-8</param>
    public static void DecryptFile(
        Stream stream,
        string key,
        string iv,
        string outputPath,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ',
        Encoding? encoding = null)
    {
        var currentEncoding = GetSafeEncoding(encoding);
        EncryptOrDecryptFile(
            stream,
            GetKeyBuffer(key, currentEncoding, fillType, fillCharacter),
            GetIvBuffer(iv, currentEncoding, fillType, fillCharacter),
            false,
            outputPath);
    }

    #endregion

    #region private methods

    private static byte[] GetKeyBuffer(string key,
        Encoding encoding,
        FillType fillType,
        char fillCharacter)
    {
        string paramName = nameof(key);
        return GetBytes(
            key,
            encoding,
            fillType,
            fillCharacter,
            32,
            () => throw new ArgumentException($"Please enter a 32-bit AES key or allow {nameof(fillType)} to Left or Right", paramName));
    }

    private static byte[] GetIvBuffer(string iv,
        Encoding encoding,
        FillType fillType,
        char fillCharacter)
    {
        string paramName = nameof(iv);
        return GetBytes(
            iv,
            encoding,
            fillType,
            fillCharacter,
            16,
            () => throw new ArgumentException($"Please enter a 16-bit iv or allow {nameof(fillType)} to Left or Right", paramName));
    }

    public static byte[] EncryptOrDecryptToBytes(
        byte[] dataBuffers,
        byte[] keyBuffer,
        byte[] ivBuffer,
        bool isEncrypt)
    {
        using var aes = Aes.Create();
        var transform = GetTransform(aes, keyBuffer, ivBuffer, isEncrypt);
        return transform.TransformFinalBlock(dataBuffers, 0, dataBuffers.Length);
    }

    /// <summary>
    /// Decrypt the stream
    /// </summary>
    /// <param name="stream">stream to be decrypted</param>
    /// <param name="keyBuffer">Decryption key, 32-bit length</param>
    /// <param name="ivBuffer">16-bit length</param>
    /// <param name="isEncrypt">encrypt (true) or decrypt (false)</param>
    /// <returns>returns the decrypted byte array</returns>
    private static byte[] EncryptOrDecryptToBytes(
        Stream stream,
        byte[] keyBuffer,
        byte[] ivBuffer,
        bool isEncrypt)
    {
        using var aes = Aes.Create();
        var transform = GetTransform(aes, keyBuffer, ivBuffer, isEncrypt);
        using MemoryStream ms = new MemoryStream();
        var cryptoStream = new CryptoStream(ms, transform, CryptoStreamMode.Write);
        var cipherBytes = stream.ConvertToBytes();
        cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
        cryptoStream.FlushFinalBlock();
        cryptoStream.Close();
        return ms.ToArray();
    }

    private static void EncryptOrDecryptFile(
        Stream stream,
        byte[] keyBuffer,
        byte[] ivBuffer,
        bool isEncrypt,
        string outputPath)
    {
        using var fileStreamOut = new FileStream(outputPath, FileMode.Create);
        using var aes = Aes.Create();
        var transform = GetTransform(aes, keyBuffer, ivBuffer, isEncrypt);
        EncryptOrDecryptFile(stream, fileStreamOut, transform);
    }

    #endregion

}
