// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

public class EncryptBase
{
    protected EncryptBase() { }

    protected static byte[] GetBytes(string str,
        Encoding encoding,
        FillType fillType,
        char fillCharacter,
        int length,
        Action action)
    {
        var result = str.GetSpecifiedLengthString(
            length,
            action,
            fillType,
            fillCharacter);
        return result.ConvertToBytes(encoding);
    }

    protected static void EncryptOrDecryptFile(
        Stream stream,
        Stream outPutStream,
        ICryptoTransform transform)
    {
        int bufferLength = 1024;
        byte[] buffers = new byte[bufferLength];
        long readLength = 0;
        using var cryptoStream = new CryptoStream(outPutStream,
            transform,
            CryptoStreamMode.Write);
        while (readLength < stream.Length)
        {
            var length = stream.Read(buffers, 0, bufferLength);
            cryptoStream.Write(buffers, 0, length);
            readLength += length;
        }
    }

    protected static ICryptoTransform GetTransform(
        SymmetricAlgorithm symmetricAlgorithm,
        byte[] keyBuffer,
        byte[] ivBuffer,
        bool isEncrypt)
        => isEncrypt ?
            symmetricAlgorithm.CreateEncryptor(keyBuffer, ivBuffer) :
            symmetricAlgorithm.CreateDecryptor(keyBuffer, ivBuffer);

    protected static Encoding GetSafeEncoding(Encoding? encoding = null)
        => GetSafeEncoding(() => Encoding.UTF8, encoding);

    protected static Encoding GetSafeEncoding(Func<Encoding> func, Encoding? encoding = null)
        => encoding ?? func.Invoke();
}
