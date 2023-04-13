// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Utils.Extensions.Validations.FluentValidation;

internal static class RegularHelper
{
    internal const string CHINESE = "^[\u4e00-\u9fa5]+$";
    internal const string NUMBER = "^[0-9]+$";
    internal const string LETTER = "^[a-zA-Z]+$";
    internal const string IDENTIFY = "^[a-zA-Z0-9\\.-]+$";
    internal const string LOWER_LETTER = "^[a-z]+$";
    internal const string UPPER_LETTER = "^[A-Z]+$";
    internal const string LETTER_NUMBER = "^[a-zA-Z0-9]+$";
    internal const string CHINESE_LETTER_NUMBER = "^[\u4e00-\u9fa5a-zA-Z0-9]+$";
    internal const string CHINESE_LETTER = "^[\u4e00-\u9fa5a-zA-Z]+$";
    internal const string CHINESE_LETTER_NUMBER_UNDERLINE = "^[\u4e00-\u9fa5_a-zA-Z0-9]+$";
    internal const string CHINESE_LETTER_NUMBER_SYMBOL = @"^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z0-9~!@#\$%\^&\*\(\)\+=\|\\\}\]\{\[_:;<.,>\?\/""]+$";
    internal const string CHINESE_LETTER_UNDERLINE = "^[\u4e00-\u9fa5_a-zA-Z]+$";
    internal const string IDCARD = "(^\\d{15}$)|(^\\d{17}([0-9]|X|x)$)";
    internal const string URL = "[a-zA-z]+://[^s]*";
    internal const string EMAIL = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
#pragma warning disable S2068
    internal const string PASSWORD_REGULAR = @"^\S*(?=\S{6,})(?=\S*\d)(?=\S*[A-Za-z])\S*$";
#pragma warning restore S2068

    internal const string PORT =
        "^([1-9]|[1-9]\\d|[1-9]\\d{2}|[1-9]\\d{3}|[1-5]\\d{4}|6[0-4]\\d{3}|65[0-4]\\d{2}|655[0-2]\\d|6553[0-5])$";

    #region Phone

    internal const string CN_PHONE = @"^(\+?0?86\-?)?1[345789]\d{9}$";
    internal const string US_PHONE = @"^(\+?1)?[2-9]\d{2}[2-9](?!11)\d{6}$";
    internal const string GB_PHONE = @"^(\+?44|0)7\d{9}$";

    #endregion
}
