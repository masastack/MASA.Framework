// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation;

internal static class RegularHelper
{
    internal const string CHINESE = "^\\s{0}$|^[\u4e00-\u9fa5]+$";
    internal const string NUMBER = "^\\s{0}$|^[0-9]+$";
    internal const string LETTER = "^\\s{0}$|^[a-zA-Z]+$";
    internal const string LOWER_LETTER = "^\\s{0}$|^[a-z]+$";
    internal const string UPPER_LETTER = "^\\s{0}$|^[A-Z]+$";
    internal const string LETTER_NUMBER = "^\\s{0}$|^[a-zA-Z0-9]+$";
    internal const string CHINESE_LETTER_NUMBER = "^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z0-9]+$";
    internal const string CHINESE_LETTER = "^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z]+$";
    internal const string PHONE = @"^\s{0}$|^((\+86)|(86))?(1[3-9][0-9])\d{8}$";
    internal const string IDCARD = "^\\s{0}$|(^\\d{15}$)|(^\\d{17}([0-9]|X|x)$)";
    internal const string EMAIL = @"^\s{0}$|^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
    internal const string URL = "^\\s{0}$|[a-zA-z]+://[^s]*";
    internal const string PORT = "^\\s{0}$|^([1-9]|[1-9]\\d|[1-9]\\d{2}|[1-9]\\d{3}|[1-5]\\d{4}|6[0-4]\\d{3}|65[0-4]\\d{2}|655[0-2]\\d|6553[0-5])$";
}
