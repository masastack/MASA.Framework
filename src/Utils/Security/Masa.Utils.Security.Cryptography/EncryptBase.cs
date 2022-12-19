// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

public class EncryptBase
{
    protected EncryptBase() { }

    protected static string GetSpecifiedLengthString(
        string key,
        int length,
        Action action,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ')
    {
        if (fillType == FillType.NoFile && key.Length < length)
            action.Invoke();

        var keyLength = key.Length;
        if (keyLength == length)
        {
            return key;
        }

        if (keyLength > length)
        {
            return key.Substring(0, length);
        }

        if (fillType == FillType.Left)
        {
            return key.PadLeft(length, fillCharacter);
        }

        if (fillType == FillType.Right)
        {
            return key.PadRight(length, fillCharacter);
        }

        throw new NotSupportedException($"... Unsupported {nameof(fillType)}");
    }

    protected static byte[] GetBytes(string str,
        Encoding encoding,
        FillType fillType,
        char fillCharacter,
        int length,
        Action action)
    {
        var result = GetSpecifiedLengthString(
            str,
            length,
            action,
            fillType,
            fillCharacter);
        return result.ConvertToBytes(encoding);
    }

    protected static Encoding GetSafeEncoding(Encoding? encoding = null)
        => GetSafeEncoding(() => Encoding.UTF8, encoding);

    protected static Encoding GetSafeEncoding(Func<Encoding> func, Encoding? encoding = null)
        => encoding ?? func.Invoke();
}
