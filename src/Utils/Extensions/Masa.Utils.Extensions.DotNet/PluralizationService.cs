// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public abstract class PluralizationService
{
    public CultureInfo Culture { get; protected set; }

    public abstract bool IsPlural(string word);

    public abstract bool IsSingular(string word);

    public abstract string Pluralize(string word);

    public abstract string Singularize(string word);

    /// <summary>
    /// Factory method for PluralizationService. Only support english pluralization.
    /// Please set the PluralizationService on the System.Data.Entity.Design.EntityModelSchemaGenerator
    /// to extend the service to other locales.
    /// </summary>
    /// <param name="culture">CultureInfo</param>
    /// <returns>PluralizationService</returns>
    public static PluralizationService CreateService(CultureInfo culture)
    {
        CheckUtil.CheckArgumentNull(culture, "culture");

        if (culture.TwoLetterISOLanguageName == "en")
            return new EnglishPluralizationService();

        throw new NotImplementedException($"Unsupported Locale for {culture.DisplayName}");
    }
}
