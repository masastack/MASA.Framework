// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> Chinese<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new ChineseValidator<T>());

    public static IRuleBuilderOptions<T, string> Number<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new NumberValidator<T>());

    public static IRuleBuilderOptions<T, string> Letter<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new LetterValidator<T>());

    public static IRuleBuilderOptions<T, string> LowerLetter<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new LowerLetterValidator<T>());

    public static IRuleBuilderOptions<T, string> UpperLetter<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new UpperLetterValidator<T>());

    public static IRuleBuilderOptions<T, string> LetterNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new LetterNumberValidator<T>());

    public static IRuleBuilderOptions<T, string> ChineseLetterUnderline<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new ChineseLetterUnderlineValidator<T>());

    public static IRuleBuilderOptions<T, string> ChineseLetter<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new ChineseLetterValidator<T>());

    public static IRuleBuilderOptions<T, string> ChineseLetterNumberUnderline<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new ChineseLetterNumberUnderlineValidator<T>());

    public static IRuleBuilderOptions<T, string> ChineseLetterNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new ChineseLetterNumberValidator<T>());

    public static IRuleBuilderOptions<T, string> Phone<T>(this IRuleBuilder<T, string> ruleBuilder, string? culture = null)
        => ruleBuilder.SetValidator(new PhoneValidator<T>(culture));

    /// <summary>
    /// For the time being, it can only be used for the verification of ID card (China)
    /// </summary>
    /// <param name="ruleBuilder"></param>
    /// <param name="culture"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> IdCard<T>(this IRuleBuilder<T, string> ruleBuilder, string? culture = null)
        => ruleBuilder.SetValidator(new IdCardValidator<T>(culture));

    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new EmailRegularValidator<T>());

    public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new UrlValidator<T>());

    public static IRuleBuilderOptions<T, string> Port<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new PortValidator<T>());

    public static IRuleBuilderOptions<T, TProperty> Required<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        => ruleBuilder.SetValidator(new RequiredValidator<T, TProperty>());

    public static IRuleBuilderOptions<T, string> Identity<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new IdentityValidator<T>());

    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder, string expression = RegularHelper.PASSWORD_REGULAR)
        => ruleBuilder.SetValidator(new PasswordValidator<T>(expression));
}
