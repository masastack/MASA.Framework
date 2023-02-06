// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Utils.Extensions.Validations.FluentValidation;

internal static class RegularHelper
{
    internal const string CHINESE = "^\\s{0}$|^[\u4e00-\u9fa5]+$";
    internal const string NUMBER = "^\\s{0}$|^[0-9]+$";
    internal const string LETTER = "^\\s{0}$|^[a-zA-Z]+$";
    internal const string IDENTIFY = "^\\s{0}$|^[a-zA-Z0-9\\.-]+$";
    internal const string LOWER_LETTER = "^\\s{0}$|^[a-z]+$";
    internal const string UPPER_LETTER = "^\\s{0}$|^[A-Z]+$";
    internal const string LETTER_NUMBER = "^\\s{0}$|^[a-zA-Z0-9]+$";
    internal const string CHINESE_LETTER_NUMBER = "^\\s{0}$|^[\u4e00-\u9fa5a-zA-Z0-9]+$";
    internal const string CHINESE_LETTER = "^\\s{0}$|^[\u4e00-\u9fa5a-zA-Z]+$";
    internal const string CHINESE_LETTER_NUMBER_UNDERLINE = "^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z0-9]+$";
    internal const string CHINESE_LETTER_UNDERLINE = "^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z]+$";
    internal const string IDCARD = "^\\s{0}$|(^\\d{15}$)|(^\\d{17}([0-9]|X|x)$)";
    internal const string URL = "^\\s{0}$|[a-zA-z]+://[^s]*";
    internal const string EMAIL = @"^\s{0}$|^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
    internal const string PASSWORD_REGULAR = @"^\s{0}$|^\S*(?=\S{6,})(?=\S*\d)(?=\S*[A-Za-z])\S*$";

    internal const string PORT =
        "^\\s{0}$|^([1-9]|[1-9]\\d|[1-9]\\d{2}|[1-9]\\d{3}|[1-5]\\d{4}|6[0-4]\\d{3}|65[0-4]\\d{2}|655[0-2]\\d|6553[0-5])$";

    #region Phone

    internal const string CN_PHONE = @"^(\+?0?86\-?)?1[345789]\d{9}$";
    internal const string US_PHONE = @"^(\+?1)?[2-9]\d{2}[2-9](?!11)\d{6}$";
    internal const string GB_PHONE = @"^(\+?44|0)7\d{9}$";

    #endregion
}
