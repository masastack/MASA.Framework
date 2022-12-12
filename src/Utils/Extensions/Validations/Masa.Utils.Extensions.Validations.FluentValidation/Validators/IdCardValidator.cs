// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Validators;

public class IdCardValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(IdCardValidator<T>);

    private readonly string? _culture;

    public IdCardValidator(string? culture) => _culture = culture;

    public override bool IsValid(ValidationContext<T> context, string value)
        => GetIIdCardProvider().IsValid(value);

    private IIdCardProvider GetIIdCardProvider()
    {
        string culture = _culture ?? GlobalValidationOptions.DefaultCulture;
        switch (culture)
        {
            case { } c when c.Equals("zh-CN", StringComparison.OrdinalIgnoreCase):
                return new ChinaIdCardProvider();
            default:
                throw new NotSupportedException($"IdCard verification in the [{culture}] is not currently supported");
        }
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
        => Localized(errorCode, Name);
}
