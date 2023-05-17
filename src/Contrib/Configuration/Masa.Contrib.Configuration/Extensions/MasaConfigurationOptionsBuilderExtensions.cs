// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class MasaConfigurationOptionsBuilderExtensions
{
    public static void UseMasaOptions(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        Action<MasaConfigurationRelationOptions> configure)
    {
        var masaConfigurationRelationOptions = new MasaConfigurationRelationOptions(masaConfigurationOptionsBuilder.AutoMapOptionsByManual);
        configure.Invoke(masaConfigurationRelationOptions);
    }
}
