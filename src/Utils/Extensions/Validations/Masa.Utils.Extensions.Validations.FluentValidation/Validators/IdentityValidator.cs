// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace FluentValidation.Validators;

public class IdentityValidator<T> : MasaRegularExpressionValidator<T>
{
    public override string Name => nameof(IdentityValidator<T>);

    public IdentityValidator() : base(RegularHelper.IDENTIFY)
    {
    }
}
