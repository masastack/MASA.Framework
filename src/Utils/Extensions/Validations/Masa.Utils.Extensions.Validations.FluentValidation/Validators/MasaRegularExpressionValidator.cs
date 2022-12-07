// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Validators;

public abstract class MasaRegularExpressionValidator<T> : RegularExpressionValidator<T>
{
    protected MasaRegularExpressionValidator(string expression) : base(expression)
    {
    }

    protected MasaRegularExpressionValidator(Regex regex) : base(regex)
    {
    }

    protected MasaRegularExpressionValidator(string expression, RegexOptions options) : base(expression, options)
    {
    }

    protected MasaRegularExpressionValidator(Func<T, string> expressionFunc) : base(expressionFunc)
    {
    }

    protected MasaRegularExpressionValidator(Func<T, Regex> regexFunc) : base(regexFunc)
    {
    }

    protected MasaRegularExpressionValidator(Func<T, string> expression, RegexOptions options) : base(expression, options)
    {
    }
}
