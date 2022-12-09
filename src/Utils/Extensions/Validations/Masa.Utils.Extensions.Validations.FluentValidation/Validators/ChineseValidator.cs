// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Validators;

public class ChineseValidator<T> : MasaRegularExpressionValidator<T>
{
    public override string Name => nameof(ChineseValidator<T>);

    public ChineseValidator() : base(RegularHelper.CHINESE)
    {
    }
}
