// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Xml.Linq;

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class IdentityValidatorTest : ValidatorBaseTest
{
    public override string Message => "'Identity' must be numbers, letters or . and - .";

    [DataRow("Masa团队", false)]
    [DataRow("masastack", true)]
    [DataRow("masastack@123", false)]
    [DataRow("Masa", true)]
    [DataRow("123#", false)]
    [DataRow("123.", true)]
    [DataRow(null, true)]
    [DataRow("", false)]
    [DataTestMethod]
    public void TestIdentity(string? identity, bool expectedResult)
    {
        var validator = new IdentityValidator();
        var result = validator.Validate(new RegisterUserEvent()
        {
            Identity = identity,
        });
        Assert.AreEqual(expectedResult, result.IsValid);
        if (!expectedResult)
        {
            Assert.AreEqual(Message, result.Errors[0].ErrorMessage);
        }
    }

    public class IdentityValidator : MasaAbstractValidator<RegisterUserEvent>
    {
        public IdentityValidator()
        {
            RuleFor(r => r.Identity).Identity();
        }
    }
}
