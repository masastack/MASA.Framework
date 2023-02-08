// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class NumberValidatorTest : ValidatorBaseTest
{
    public override string Message => "'Id Card' must be Number.";

    [DataRow("团队", false)]
    [DataRow("Masa团队", false)]
    [DataRow("masastack", false)]
    [DataRow("masastack123", false)]
    [DataRow("masa", false)]
    [DataRow("MASA", false)]
    [DataRow("Masa", false)]
    [DataRow("Masa123", false)]
    [DataRow("123", true)]
    [DataRow("123.", false)]
    [DataTestMethod]
    public void TestNumber(string idCard, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator();
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

    public class RegisterUserEventValidator : AbstractValidator<RegisterUserEvent>
    {
        public RegisterUserEventValidator()
        {
            RuleFor(r => r.IdCard).Number();
        }
    }
}
