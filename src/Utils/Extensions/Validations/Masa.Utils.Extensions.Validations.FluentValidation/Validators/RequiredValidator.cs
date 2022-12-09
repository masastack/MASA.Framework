// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Validators;

public class RequiredValidator<T, TProperty> : NotEmptyValidator<T, TProperty>
{
    public override string Name => nameof(RequiredValidator<T, TProperty>);
}
