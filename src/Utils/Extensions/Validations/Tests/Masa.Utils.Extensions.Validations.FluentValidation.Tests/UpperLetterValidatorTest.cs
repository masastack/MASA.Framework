// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class UpperLetterValidatorTest
{
    [DataRow("团队", false)]
    [DataRow("Masa团队", false)]
    [DataRow("masastack", false)]
    [DataRow("123", false)]
    [DataRow("masastack123", false)]
    [DataRow("masa", false)]
    [DataRow("MASA", true)]
    [DataRow("Masa", false)]
    [DataTestMethod]
    public void TestLowerLetter(string name, bool expectedResult)
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
            RuleFor(r => r.Name).UpperLetter();
        }
    }
}
