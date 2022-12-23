// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.Configuration;

public interface IMasaConfigurationBuilder : IConfigurationBuilder
{
    IServiceCollection Services { get; }

    IConfiguration Configuration { get; }

    void AddRepository(IConfigurationRepository configurationRepository);

    void AddRelations(params ConfigurationRelationOptions[] relationOptions);
}
