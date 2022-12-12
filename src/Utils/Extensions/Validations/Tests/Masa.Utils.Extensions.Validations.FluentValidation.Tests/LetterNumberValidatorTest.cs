// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class LetterNumberValidatorTest : ValidatorBaseTest
{
    public override string Message => "'Name' must be a letter or underscore.";

    [DataRow("团队", false)]
    [DataRow("Masa团队", false)]
    [DataRow("masastack", true)]
    [DataRow("123", true)]
    [DataRow("masastack123", true)]
    [DataRow("团队123", false)]
    [DataTestMethod]
    public void TestLetterNumber(string name, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator();
        var result = validator.Validate(new RegisterUserEvent()
        {
            Name = name
        });
        Assert.AreEqual(expectedResult, result.IsValid);
        if (!expectedResult)
        {
            Assert.AreEqual(Message, result.Errors[0].ErrorMessage);
        }
    }

    public class RegisterUserEventValidator : AbstractValidator<RegisterUserEvent>
    {
        public RegisterUserEventValidator()
        {
            RuleFor(r => r.Name).LetterNumber();
        }
    }
}
