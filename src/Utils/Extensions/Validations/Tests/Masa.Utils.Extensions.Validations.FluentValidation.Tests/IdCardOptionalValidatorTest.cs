// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class IdCardOptionalValidatorTest : ValidatorBaseTest
{
    public override string Message => "'Id Card' is not a valid ID.";

    [DataRow("410785195212123541", false)]
    [DataRow("110101192803011819", true)]
    [DataRow("", true)]
    [DataRow(null, true)]
    [DataTestMethod]
    public void TestIdCard(string? idCard, bool expectedResult)
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

    [DataRow(null)]
    [DataRow("110101192803011819")]
    [DataRow("")]
    [TestMethod]
    public void TestIdCardByUs(string? idCard)
    {
        var validator = new RegisterUserEventValidator("en-US");
        switch (idCard)
        {
            case null:
            case "":
                var result = validator.Validate(new RegisterUserEvent()
                {
                    IdCard = idCard
                });
                Assert.IsTrue(result.IsValid);
                break;
            default:
                Assert.ThrowsExactly<NotSupportedException>(() => validator.Validate(new RegisterUserEvent()
                {
                    IdCard = idCard
                }));
                break;
        }
    }

    public class RegisterUserEventValidator : MasaAbstractValidator<RegisterUserEvent>
    {
        public RegisterUserEventValidator(string? culture = null)
        {
            _ = WhenNotEmpty(r => r.IdCard, new IdCardValidator<RegisterUserEvent>(culture));
        }
    }
}
