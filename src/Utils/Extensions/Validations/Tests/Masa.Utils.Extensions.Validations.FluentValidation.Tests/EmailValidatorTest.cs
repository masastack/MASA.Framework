// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class EmailValidatorTest : ValidatorBaseTest
{
    public override string Message => "'Email' must be a legal Email.";

    [DataRow("masastack123@", false)]
    [DataRow("123", false)]
    [DataRow("123@qq.com", true)]
    [DataTestMethod]
    public void TestEmail(string email, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator();
        var result = validator.Validate(new RegisterUserEvent()
        {
            Email = email
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
            RuleFor(r => r.Email).Email();
        }
    }
}
