// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class LetterValidatorTest
{
    [DataRow("团队", false)]
    [DataRow("Masa团队", false)]
    [DataRow("masastack", true)]
    [DataRow("123", false)]
    [DataRow("masastack123", false)]
    [DataRow("Masa", true)]
    [DataTestMethod]
    public void TestLetterNumber(string name, bool expectedResult)
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
            RuleFor(r => r.Name).Letter();
        }
    }
}
