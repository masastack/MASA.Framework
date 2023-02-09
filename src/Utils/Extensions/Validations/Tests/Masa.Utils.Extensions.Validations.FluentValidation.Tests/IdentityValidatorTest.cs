// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class IdentityValidatorTest : ValidatorBaseTest
{
    public override string Message => "'' must be numbers, letters or . and - .";

    [DataRow("Masa团队", false)]
    [DataRow("masastack", true)]
    [DataRow("masastack@123", false)]
    [DataRow("Masa", true)]
    [DataRow("123#", false)]
    [DataRow("123.", true)]
    [DataTestMethod]
    public void TestIdentity(string identity, bool expectedResult)
    {
        var validator = new IdentityValidator();
        var result = validator.Validate(identity);
        Assert.AreEqual(expectedResult, result.IsValid);
        if (!expectedResult)
        {
            Assert.AreEqual(Message, result.Errors[0].ErrorMessage);
        }
    }

    public class IdentityValidator : AbstractValidator<string>
    {
        public IdentityValidator()
        {
            RuleFor(r => r).Identity();
        }
    }
}
