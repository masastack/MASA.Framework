﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18n;

public static class I18n
{
    private static readonly IServiceProvider _serviceProvider = MasaApp.RootServiceProvider;
    private static readonly II18n _i18n = InitI18n();
    private static readonly ILanguageProvider _languageProvider = InitLanguage();

    static II18n InitI18n() => _serviceProvider.GetRequiredService<II18n>();

    static ILanguageProvider InitLanguage() => _serviceProvider.GetRequiredService<ILanguageProvider>();

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <returns></returns>
    public static string T(string name) => _i18n.T(name);

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <returns></returns>
    public static string? T(string name, bool returnKey) => _i18n.T(name, returnKey);

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="arguments">The values to format the string with.</param>
    public static string T(string name, params object[] arguments) => _i18n.T(name, arguments);

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <param name="arguments">The values to format the string with.</param>
    public static string? T(string name, bool returnKey, params object[] arguments) => _i18n.T(name, returnKey, arguments);

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <typeparam name="TResource">Customer Resource Type</typeparam>
    /// <returns></returns>
    public static string T<TResource>(string name)
        => _serviceProvider.GetRequiredService<II18n<TResource>>().T(name);

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="propertyExpression">attribute expression.</param>
    /// <typeparam name="TResource">Customer Resource Type</typeparam>
    /// <returns></returns>
    public static string T<TResource>(Expression<Func<TResource, string>> propertyExpression)
        => T<TResource>(ExpressionExtensions.GetI18nName(propertyExpression));

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <typeparam name="TResource">Customer Resource Type</typeparam>
    /// <returns></returns>
    public static string? T<TResource>(string name, bool returnKey)
        => _serviceProvider.GetRequiredService<II18n<TResource>>().T(name, returnKey);

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="propertyExpression">attribute expression.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <typeparam name="TResource">Customer Resource Type</typeparam>
    /// <returns></returns>
    public static string? T<TResource>(Expression<Func<TResource, string>> propertyExpression, bool returnKey)
        => T<TResource>(ExpressionExtensions.GetI18nName(propertyExpression), returnKey);

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="arguments">The values to format the string with.</param>
    /// <typeparam name="TResource">Customer Resource Type</typeparam>
    public static string T<TResource>(string name, params object[] arguments)
        => _serviceProvider.GetRequiredService<II18n<TResource>>().T(name, arguments);

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>>
    /// <param name="propertyExpression">attribute expression.</param>
    /// <param name="arguments">The values to format the string with.</param>
    /// <typeparam name="TResource">Customer Resource Type</typeparam>
    public static string T<TResource>(Expression<Func<TResource, string>> propertyExpression, params object[] arguments)
        => T<TResource>(ExpressionExtensions.GetI18nName(propertyExpression), arguments);

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <param name="arguments">The values to format the string with.</param>
    /// <typeparam name="TResource">Customer Resource Type</typeparam>
    public static string? T<TResource>(string name, bool returnKey, params object[] arguments)
        => _serviceProvider.GetRequiredService<II18n<TResource>>().T(name, returnKey, arguments);

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    /// <param name="propertyExpression">attribute expression.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <param name="arguments">The values to format the string with.</param>
    /// <typeparam name="TResource">Customer Resource Type</typeparam>
    public static string? T<TResource>(Expression<Func<TResource, string>> propertyExpression, bool returnKey, params object[] arguments)
        => T<TResource>(ExpressionExtensions.GetI18nName(propertyExpression), returnKey, arguments);

    public static CultureInfo GetCultureInfo() => _i18n.GetCultureInfo();

    /// <summary>
    /// Set the CultureName
    /// Data used to define "regional options", standards, formats, etc
    /// </summary>
    /// <param name="cultureName">A predefined <see cref="T:System.Globalization.CultureInfo" /> name, <see cref="P:System.Globalization.CultureInfo.Name" /> of an existing <see cref="T:System.Globalization.CultureInfo" />, or Windows-only culture name. <paramref name="name" /> is not case-sensitive.</param>
    /// <param name="useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (<see langword="true" />) or the default culture settings (<see langword="false" />).</param>
    public static void SetCulture(string cultureName, bool useUserOverride = true) => _i18n.SetCulture(cultureName, useUserOverride);

    /// <summary>
    /// Set the CultureName
    /// Data used to define "regional options", standards, formats, etc
    /// </summary>
    /// <param name="culture"></param>
    public static void SetCulture(CultureInfo culture) => _i18n.SetCulture(culture);

    /// <summary>
    /// get interface language
    /// </summary>
    /// <returns></returns>
    public static CultureInfo GetUiCultureInfo() => _i18n.GetUiCultureInfo();

    /// <summary>
    /// Set the CultureName for the current request
    /// Used to set the interface language
    /// </summary>
    /// <param name="cultureName">A predefined <see cref="T:System.Globalization.CultureInfo" /> name, <see cref="P:System.Globalization.CultureInfo.Name" /> of an existing <see cref="T:System.Globalization.CultureInfo" />, or Windows-only culture name. <paramref name="name" /> is not case-sensitive.</param>
    /// <param name="useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (<see langword="true" />) or the default culture settings (<see langword="false" />).</param>
    public static void SetUiCulture(string cultureName, bool useUserOverride = true) => _i18n.SetUiCulture(cultureName, useUserOverride);

    /// <summary>
    /// Set the CultureName for the current request
    /// Used to set the interface language
    /// </summary>
    /// <param name="culture"></param>
    public static void SetUiCulture(CultureInfo culture) => _i18n.SetUiCulture(culture);

    public static IReadOnlyList<LanguageInfo> GetLanguages() => _languageProvider.GetLanguages();
}
