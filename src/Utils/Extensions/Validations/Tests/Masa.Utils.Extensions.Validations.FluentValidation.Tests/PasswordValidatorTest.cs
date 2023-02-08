// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class PasswordValidatorTest : ValidatorBaseTest
{
    public override string Message => "'Password' password validation rule failed.";

    [DataRow("Masa团队", false)]
    [DataRow("masastack", false)]
    [DataRow("masastack@123", true)]
    [DataRow("Masa", false)]
    [DataRow("Masa@123", true)]
    [DataRow("123", false)]
    [DataRow("123.", false)]
    [DataTestMethod]
    public void TestPassword(string pwd, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator();
        var result = validator.Validate(new RegisterUserEvent()
        {
            Password = pwd
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
            RuleFor(r => r.Password).Password();
        }
    }
}
