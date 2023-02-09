// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace FluentValidation.Validators;

public class EmailRegularValidator<T> : MasaRegularExpressionValidator<T>
{
    public override string Name => nameof(EmailRegularValidator<T>);

    public EmailRegularValidator() : base(RegularHelper.EMAIL)
    {
    }
}
