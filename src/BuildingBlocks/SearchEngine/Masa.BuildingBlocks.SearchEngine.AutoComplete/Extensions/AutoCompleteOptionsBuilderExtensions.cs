// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class AutoCompleteOptionsBuilderExtensions
{
    public static void UseCustomAutoComplete(
        this AutoCompleteOptionsBuilder autoCompleteOptionsBuilder,
        Func<IServiceProvider, IManualAutoCompleteClient> implementationFactory)
    {
        MasaArgumentException.ThrowIfNull(implementationFactory);

        autoCompleteOptionsBuilder.Services.Configure<AutoCompleteFactoryOptions>(factoryOptions =>
        {
            if (factoryOptions.Options.Any(relation => relation.Name == autoCompleteOptionsBuilder.Name))
                throw new ArgumentException(
                    $"The {nameof(IAutoCompleteClient)} name already exists, please change the name, the repeat name is [{autoCompleteOptionsBuilder.Name}]");

            factoryOptions.Options.Add(new MasaRelationOptions<IManualAutoCompleteClient>(autoCompleteOptionsBuilder.Name, implementationFactory));
        });
    }
}
