// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class PortValidatorTest: ValidatorBaseTest
{
    public override string Message => "'Port' must be a legal port, it needs to be between [0 - 65535].";

    [DataRow("10", true)]
    [DataRow("-10", false)]
    [DataRow("65535", true)]
    [DataRow("65536", false)]
    [DataTestMethod]
    public void TestPhone(string port, bool expectedResult)
    {
        var validator = new RegisterPortEventValidator();
        var result = validator.Validate(new RegisterPortEvent()
        {
            Port = port
        });
        Assert.AreEqual(expectedResult, result.IsValid);
        if (!expectedResult)
        {
            Assert.AreEqual(Message, result.Errors[0].ErrorMessage);
        }
    }

    public class RegisterPortEventValidator : MasaAbstractValidator<RegisterPortEvent>
    {
        public RegisterPortEventValidator()
        {
            RuleFor(r => r.Port).Port();
        }
    }
}
