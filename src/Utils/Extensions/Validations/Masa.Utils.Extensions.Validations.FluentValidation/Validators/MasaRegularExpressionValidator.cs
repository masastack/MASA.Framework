// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Validators;

public abstract class MasaRegularExpressionValidator<T> : RegularExpressionValidator<T>
{
    protected MasaRegularExpressionValidator(string expression) : base(expression)
    {
    }
}
