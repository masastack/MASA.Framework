// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public static class I18n
{
    private static readonly IServiceProvider _serviceProvider = MasaApp.RootServiceProvider;
    private static readonly II18N _i18N = InitI18N();
    private static readonly ILanguageProvider _languageProvider = InitLanguage();

    static II18N InitI18N() => _serviceProvider.GetRequiredService<II18N>();

    static ILanguageProvider InitLanguage() => _serviceProvider.GetRequiredService<ILanguageProvider>();

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <returns></returns>
    public static string T(string name) => _i18N.T(name);

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <returns></returns>
    public static string? T(string name, bool returnKey) => _i18N.T(name, returnKey);

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="arguments">The values to format the string with.</param>
    public static string T(string name, params object[] arguments) => _i18N.T(name, arguments);

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <param name="arguments">The values to format the string with.</param>
    public static string? T(string name, bool returnKey, params object[] arguments) => _i18N.T(name, returnKey, arguments);

    public static CultureInfo GetCultureInfo() => _i18N.GetCultureInfo();

    /// <summary>
    /// Set the CultureName
    /// Data used to define "regional options", standards, formats, etc
    /// </summary>
    /// <param name="cultureName">A predefined <see cref="T:System.Globalization.CultureInfo" /> name, <see cref="P:System.Globalization.CultureInfo.Name" /> of an existing <see cref="T:System.Globalization.CultureInfo" />, or Windows-only culture name. <paramref name="name" /> is not case-sensitive.</param>
    /// <param name="useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (<see langword="true" />) or the default culture settings (<see langword="false" />).</param>
    public static void SetCulture(string cultureName, bool useUserOverride = true) => _i18N.SetCulture(cultureName, useUserOverride);

    /// <summary>
    /// Set the CultureName
    /// Data used to define "regional options", standards, formats, etc
    /// </summary>
    /// <param name="culture"></param>
    public static void SetCulture(CultureInfo culture) => _i18N.SetCulture(culture);

    /// <summary>
    /// get interface language
    /// </summary>
    /// <returns></returns>
    public static CultureInfo GetUiCultureInfo() => _i18N.GetUiCultureInfo();

    /// <summary>
    /// Set the CultureName for the current request
    /// Used to set the interface language
    /// </summary>
    /// <param name="cultureName">A predefined <see cref="T:System.Globalization.CultureInfo" /> name, <see cref="P:System.Globalization.CultureInfo.Name" /> of an existing <see cref="T:System.Globalization.CultureInfo" />, or Windows-only culture name. <paramref name="name" /> is not case-sensitive.</param>
    /// <param name="useUserOverride">A Boolean that denotes whether to use the user-selected culture settings (<see langword="true" />) or the default culture settings (<see langword="false" />).</param>
    public static void SetUiCulture(string cultureName, bool useUserOverride = true) => _i18N.SetUiCulture(cultureName, useUserOverride);

    /// <summary>
    /// Set the CultureName for the current request
    /// Used to set the interface language
    /// </summary>
    /// <param name="culture"></param>
    public static void SetUiCulture(CultureInfo culture) => _i18N.SetUiCulture(culture);

    public static IReadOnlyList<LanguageInfo> GetLanguages() => _languageProvider.GetLanguages();
}
