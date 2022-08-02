// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Cryptography;

public class EncryptBase
{
    protected static string GetSpecifiedLengthString(
        string key,
        int length,
        Func<Exception> func,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ')
    {
        if (fillType == FillType.NoFile && key.Length < length)
        {
            throw func.Invoke();
        }

        if (key.Length >= length)
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

    protected static Encoding GetSafeEncoding(Encoding? encoding = null)
        => GetSafeEncoding(() => Encoding.UTF8, encoding);

    protected static Encoding GetSafeEncoding(Func<Encoding> func, Encoding? encoding = null)
        => encoding ?? func.Invoke();
}
