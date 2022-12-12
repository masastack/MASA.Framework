// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class RequiredValidatorTest: ValidatorBaseTest
{
    public override string Message => "'Remark' is required.";

    [DataRow("团队", true)]
    [DataRow("Masa团队", true)]
    [DataRow("masastack", true)]
    [DataRow("123", true)]
    [DataRow("masastack123", true)]
    [DataRow("masa", true)]
    [DataRow("MASA", true)]
    [DataRow("Masa", true)]
    [DataRow("https://github.com/masastack", true)]
    [DataRow("http://github.com/masastack", true)]
    [DataRow("github.com", true)]
    [DataRow("", false)]
    [DataRow(" ", false)]
    [DataRow(null, false)]
    [DataTestMethod]
    public void TestLowerLetter(string? remark, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator();
        var result = validator.Validate(new RegisterUserEvent()
        {
            Remark = remark
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
            RuleFor(r => r.Remark).Required();
        }
    }
}
