// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class LetterNumberUnderlineValidatorTest : ValidatorBaseTest
{
    public override string Message => "'Identity' must be numbers, letters or underscores.";

    [DataRow("团队123", false)]
    [DataRow("Masa团队", false)]
    [DataRow("masastack", true)]
    [DataRow("123", true)]
    [DataRow("masastack123", true)]
    [DataRow(".", false)]
    [DataRow("123.", false)]
    [DataRow("123_", true)]
    [DataRow(null, true)]
    [DataRow("", false)]
    [DataTestMethod]
    public void TestLetterNumberUnderline(string identity, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator();
        var result = validator.Validate(new RegisterUserEvent()
        {
            Identity = identity
        });
        Assert.AreEqual(expectedResult, result.IsValid);
        if (!expectedResult)
        {
            Assert.AreEqual(Message, result.Errors[0].ErrorMessage);
        }
    }

    public class RegisterUserEventValidator : MasaAbstractValidator<RegisterUserEvent>
    {
        public RegisterUserEventValidator()
        {
            RuleFor(r => r.Identity).LetterNumberUnderline();
        }
    }
}
