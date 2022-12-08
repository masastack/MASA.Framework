// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ChinaIdCardProvider : IIdCardProvider
{
    public bool IsValid(string cardNo)
    {
        if (string.IsNullOrEmpty(cardNo))
            return false;
        if (cardNo.Length == 18)
            return CheckIdCard18(cardNo);
        if (cardNo.Length == 15)
            return CheckIdCard15(cardNo);

        return false;
    }

    private static bool CheckIdCard18(string id)
    {
        if (long.TryParse(id.Remove(17), out var n) == false || n < Math.Pow(10, 16) ||
            long.TryParse(id.Replace('x', '0').Replace('X', '0'), out n) == false)
            return false;

        string address =
            "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
        if (address.IndexOf(id.Remove(2), StringComparison.Ordinal) == -1)
            return false;

        string birth = id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
        if (DateTime.TryParse(birth, out _) == false)
            return false;

        string[] arrVerifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
        string[] wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
        char[] ai = id.Remove(17).ToCharArray();
        int sum = 0;
        for (int i = 0; i < 17; i++)
            sum += int.Parse(wi[i]) * int.Parse(ai[i].ToString());

        Math.DivRem(sum, 11, out var y);
        if (arrVerifyCode[y] != id.Substring(17, 1).ToLower())
            return false;

        return true;
    }

    private static bool CheckIdCard15(string id)
    {
        if (long.TryParse(id, out var n) == false || n < Math.Pow(10, 14))
            return false;

        string address =
            "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
        if (address.IndexOf(id.Remove(2), StringComparison.Ordinal) == -1)
            return false;

        string birth = id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
        if (DateTime.TryParse(birth, out _) == false)
            return false;

        return true;
    }
}
