// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class ChineseLetterValidatorTest
{
    [DataRow("团队", true)]
    [DataRow("Masa团队", true)]
    [DataRow("masastack", true)]
    [DataRow("123", false)]
    [DataRow("masastack123", false)]
    [DataRow("团队123", false)]
    [DataTestMethod]
    public void TestChineseLetter(string name, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator();
        var result = validator.Validate(new RegisterUserEvent()
        {
            Name = name
        });
        Assert.AreEqual(expectedResult, result.IsValid);
    }

    public class RegisterUserEventValidator : AbstractValidator<RegisterUserEvent>
    {
        public RegisterUserEventValidator()
        {
            RuleFor(r => r.Name).ChineseLetter();
        }
    }
}
