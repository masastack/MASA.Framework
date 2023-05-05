// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerBuilderExtensions
{
    public static void UseCustomCaller(
        this CallerBuilder callerBuilder,
        Func<IServiceProvider, IManualCaller> implementationFactory)
    {
        callerBuilder.Services.Configure<CallerFactoryOptions>(factoryOptions =>
        {
            if (factoryOptions.Options.Any(relation => relation.Name.Equals(callerBuilder.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The caller name already exists, please change the name, the repeat name is [{callerBuilder.Name}]");

            factoryOptions.Options.Add(new MasaRelationOptions<IManualCaller>(
                callerBuilder.Name,
                implementationFactory));
        });
    }
}
