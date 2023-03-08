// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Extensions.Validations.FluentValidation.Tests;

[TestClass]
public class PhoneOptionalValidatorTest : ValidatorBaseTest
{
    public override string Message => "'Phone' must be a valid mobile phone number.";

    [DataRow("13677777777", "zh-CN", true)]
    [DataRow("8613677777777", "zh-CN", true)]
    [DataRow("+8613677777777", "zh-CN", true)]
    [DataRow("8613677777777", "zh-CN", true)]
    [DataRow("18613677777777", "zh-CN", false)]
    [DataRow(null, "zh-CN", true)]
    [DataRow("", "zh-CN", true)]
    [DataRow("+19104521452", "en-US", true)]
    [DataRow("8613677777777", "en-US", false)]
    [DataRow("+11021021521", "en-US", false)]
    [DataTestMethod]
    public void TestPhone(string phone, string? culture, bool expectedResult)
    {
        var validator = new RegisterUserEventValidator(culture);
        var result = validator.Validate(new RegisterUserEvent()
        {
            Phone = phone
        });
        Assert.AreEqual(expectedResult, result.IsValid);
        if (!expectedResult)
        {
            Assert.AreEqual(Message, result.Errors[0].ErrorMessage);
        }
    }

    [DataRow(null)]
    [DataRow("133333")]
    [DataRow("")]
    [TestMethod]
    public void TestPhoneByJapan(string phone)
    {
        string culture = "ja-jp";
        var validator = new RegisterUserEventValidator(culture);

        switch (phone)
        {
            case null:
            case "":
                var result = validator.Validate(new RegisterUserEvent()
                {
                    Phone = phone
                });
                Assert.IsTrue(result.IsValid);
                break;
            default:
                Assert.ThrowsException<NotSupportedException>(() => validator.Validate(new RegisterUserEvent()
                {
                    Phone = phone
                }));
                break;
        }
    }

    public class RegisterUserEventValidator : MasaAbstractValidator<RegisterUserEvent>
    {
        public RegisterUserEventValidator(string? culture)
        {
            //_ = WhenNotEmpty(r => r.Phone, r => r.Phone, new PhoneValidator<RegisterUserEvent>(culture));
            //_ = WhenNotEmpty(r => r.Phone, new PhoneValidator<RegisterUserEvent>(culture));
            _ = WhenNotEmpty(r => r.Phone, rule => rule.Phone(culture));
        }
    }
}
