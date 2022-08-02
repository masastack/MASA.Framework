// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace FluentValidation;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> Chinese<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.CHINESE)
                          .WithMessage("Can only input chinese of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> Number<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.NUMBER)
                          .WithMessage("Can only input number of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> Letter<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.LETTER)
                          .WithMessage("Can only input letter of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> LowerLetter<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.LOWER_LETTER)
                          .WithMessage("Can only input lower letter of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> UpperLetter<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.UPPER_LETTER)
                          .WithMessage("Can only input upper letter of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> LetterNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.LETTER_NUMBER)
                          .WithMessage("Can only input upper letter and number of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> ChineseLetter<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.CHINESE_LETTER)
                          .WithMessage("Can only input upper chinese and letter of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> ChineseLetterNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.CHINESE_LETTER_NUMBER)
                          .WithMessage("Can only input upper chinese and letter and number of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> Phone<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches<T>(RegularHelper.PHONE)
                          .WithMessage("{PropertyName} format is incorrect");
    }

    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.EMAIL)
                          .WithMessage("{PropertyName} format is incorrect");
    }

    public static IRuleBuilderOptions<T, string> IdCard<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.IDCARD)
                          .WithMessage("{PropertyName} format is incorrect");
    }

    public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.URL)
                          .WithMessage("{PropertyName} format is incorrect");
    }

    public static IRuleBuilderOptions<T, string> MinLength<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength)
    {
        return ruleBuilder.MinimumLength(minimumLength)
                    .WithMessage("Please enter a number greater than {MinLength} of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> MaxLength<T>(this IRuleBuilder<T, string> ruleBuilder, int maximumLength)
    {
        return ruleBuilder.MaximumLength(maximumLength)
                    .WithMessage("Please enter a number less than {MaxLength} of {PropertyName}");
    }

    public static IRuleBuilderOptions<T, string> Port<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Matches(RegularHelper.PORT)
                          .WithMessage("Is not a valid port {PropertyName}");
    }

    public static IRuleBuilderOptions<T, TProperty> Required<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return ruleBuilder.NotNull()
                        .NotEmpty()
                        .WithMessage("{PropertyName} is required");
    }
}
