// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class ChineseValidatorTest: ValidatorBaseTest
{
    public override string Message => "'Name' must be Chinese.";

    [DataRow("团队", true)]
    [DataRow("Masa团队", false)]
    [DataRow("masastack", false)]
    [DataRow("123", false)]
    [DataRow("masastack123", false)]
    [DataRow("团队123", false)]
    [DataRow(null, true)]
    [DataRow("", false)]
    [DataTestMethod]
    public void TestChinese(string name, bool expectedResult)
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

    public class RegisterUserEventValidator : MasaAbstractValidator<RegisterUserEvent>
    {
        public RegisterUserEventValidator()
        {
            RuleFor(r => r.Name).Chinese();
        }
    }
}
