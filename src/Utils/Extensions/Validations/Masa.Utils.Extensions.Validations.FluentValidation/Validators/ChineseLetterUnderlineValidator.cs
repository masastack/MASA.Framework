// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Validators;

public class ChineseLetterUnderlineValidator<T> : MasaRegularExpressionValidator<T>
{
    public override string Name => nameof(ChineseLetterUnderlineValidator<T>);

    public ChineseLetterUnderlineValidator() : base(RegularHelper.CHINESE_LETTER_UNDERLINE)
    {
    }
}
