// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration;

internal class DefaultMasaConfigurationChangeListener : IMasaConfigurationChangeListener
{
    private delegate void ConfigurationChangedHandler(MasaConfigurationChangeOptions changeOptions);

    private event ConfigurationChangedHandler? ConfigurationChanged;

    public void AddChangeListener(Action<MasaConfigurationChangeOptions> action)
    {
        ConfigurationChanged += action.Invoke;
    }

    public void OnChanged(MasaConfigurationChangeOptions changeOptions)
        => ConfigurationChanged?.Invoke(changeOptions);
}
