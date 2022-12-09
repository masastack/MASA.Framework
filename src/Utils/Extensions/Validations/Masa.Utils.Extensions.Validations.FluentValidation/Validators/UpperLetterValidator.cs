// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Validators;

public class UpperLetterValidator<T> : MasaRegularExpressionValidator<T>
{
    public override string Name => nameof(UpperLetterValidator<T>);

    public UpperLetterValidator() : base(RegularHelper.UPPER_LETTER)
    {
    }
}
