// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public abstract class LocalizationResourceContributorBase : ILocalizationResourceContributor
{
    private Dictionary<string, LocalizedString>? _dictionaries;

    public Type ResourceType { get; }

    public string CultureName { get; protected set; }

    public LocalizationResourceContributorBase(Type resourceType)
    {
        ResourceType = resourceType;
    }

    public LocalizedString? GetOrNull(string name)
    {
        if (_dictionaries == null)
        {
            var item = GetCultureNameAndDictionaries();
            CultureName = item.CultureName;
            _dictionaries = item.Dictionary;
        }
        return GetOrNull(_dictionaries, name);
    }

    protected abstract (string CultureName, Dictionary<string, LocalizedString> Dictionary) GetCultureNameAndDictionaries();

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
