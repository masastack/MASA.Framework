// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

public abstract class ValidatorBaseTest
{
    public abstract string Message { get; }

    public ValidatorBaseTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        ValidatorOptions.Global.LanguageManager = new MasaLanguageManager();
    }
}
