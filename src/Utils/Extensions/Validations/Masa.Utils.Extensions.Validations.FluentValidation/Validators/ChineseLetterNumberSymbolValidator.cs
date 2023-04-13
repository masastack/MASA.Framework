// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace FluentValidation.Validators;

public class ChineseLetterNumberSymbolValidator<T> : MasaRegularExpressionValidator<T>
{
    public override string Name => nameof(ChineseLetterNumberSymbolValidator<T>);

    public ChineseLetterNumberSymbolValidator() : base(RegularHelper.CHINESE_LETTER_NUMBER_SYMBOL)
    {
    }
}
