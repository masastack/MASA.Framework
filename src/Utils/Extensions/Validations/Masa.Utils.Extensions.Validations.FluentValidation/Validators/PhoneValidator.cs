// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Validators;

public class PhoneValidator<T> : PropertyValidator<T, string>
{
    private readonly string? _culture;

    public override string Name => nameof(PhoneValidator<T>);

    public PhoneValidator(string? culture) => _culture = culture;

    public override bool IsValid(ValidationContext<T> context, string? value)
    {
        var regex = CreateRegex(GetExpression(_culture));

        if (value != null && !regex.IsMatch(value))
        {
            context.MessageFormatter.AppendArgument("RegularExpression", regex.ToString());
            return false;
        }

        if (value == null) return false;

        var result = regex.Match(value);
        return result.Value == value;
    }

    private static string GetExpression(string? culture)
    {
        switch (culture ?? GlobalValidationOptions.DefaultCulture)
        {
            case { } c when c.Equals("zh-CN", StringComparison.OrdinalIgnoreCase):
                return RegularHelper.CHINA_PHONE;
            case { } c when c.Equals("en-US", StringComparison.OrdinalIgnoreCase):
                return RegularHelper.US_PHONE;
            case { } c when c.Equals("en-GB", StringComparison.OrdinalIgnoreCase):
                return RegularHelper.GB_PHONE;
            default:
                throw new NotSupportedException($"Mobile phone number verification in the [{culture}] is not currently supported");
        }
    }

    private static Regex CreateRegex(string expression, RegexOptions options = RegexOptions.None)
        => new(expression, options, TimeSpan.FromSeconds(2.0));
}
