// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public abstract class LocalizationResourceContributorBase : ILocalizationResourceContributor
{
    private Dictionary<string, LocalizedString>? _dictionaries;

    public Type ResourceType { get; }

    public string CultureName { get; }

    public LocalizationResourceContributorBase(Type resourceType, string cultureName)
    {
        ResourceType = resourceType;
        CultureName = cultureName;
    }

    public LocalizedString? GetOrNull(string name)
    {
        _dictionaries ??= GetDictionaries();
        return GetOrNull(_dictionaries, name);
    }

    protected abstract Dictionary<string, LocalizedString> GetDictionaries();

    protected virtual LocalizedString? GetOrNull(Dictionary<string, LocalizedString> dictionaries, string name)
    {
        if (dictionaries.TryGetValue(name, out LocalizedString? value))
            return value;

        return null;
    }

    public virtual void ResetResource()
    {
        _dictionaries = null;
    }
}
