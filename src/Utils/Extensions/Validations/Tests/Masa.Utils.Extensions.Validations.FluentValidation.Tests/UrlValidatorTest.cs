// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class UrlValidatorTest
{
    [DataRow("团队", false)]
    [DataRow("Masa团队", false)]
    [DataRow("masastack", false)]
    [DataRow("123", false)]
    [DataRow("masastack123", false)]
    [DataRow("masa", false)]
    [DataRow("MASA", false)]
    [DataRow("Masa", false)]
    [DataRow("https://github.com/masastack", true)]
    [DataRow("http://github.com/masastack", true)]
    [DataRow("github.com", false)]
    [DataTestMethod]
    public void TestLowerLetter(string url, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator();
        var result = validator.Validate(new RegisterUserEvent()
        {
            Referer = url
        });
        Assert.AreEqual(expectedResult, result.IsValid);
    }

    public class RegisterUserEventValidator : AbstractValidator<RegisterUserEvent>
    {
        public RegisterUserEventValidator()
        {
            RuleFor(r => r.Referer).Url();
        }
    }
}
