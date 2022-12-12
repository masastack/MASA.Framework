﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class IdCardValidatorTest : ValidatorBaseTest
{
    public override string Message => "'Id Card' is not a valid ID.";

    [DataRow("410785195212123541", false)]
    [DataTestMethod]
    public void TestIdCard(string idCard, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator("zh-CN");
        var result = validator.Validate(new RegisterUserEvent()
        {
            IdCard = idCard
        });
        Assert.AreEqual(expectedResult, result.IsValid);
        if (!expectedResult)
        {
            Assert.AreEqual(Message, result.Errors[0].ErrorMessage);
        }
    }

    [TestMethod]
    public void TestIdCardByUs()
    {
        string idCard = "";
        var validator = new RegisterUserEventValidator("en-US");
        Assert.ThrowsException<NotSupportedException>(() => validator.Validate(new RegisterUserEvent()
        {
            IdCard = idCard
        }));
    }

    public class RegisterUserEventValidator : AbstractValidator<RegisterUserEvent>
    {
        public RegisterUserEventValidator(string culture)
        {
            RuleFor(r => r.IdCard).IdCard(culture);
        }
    }
}
