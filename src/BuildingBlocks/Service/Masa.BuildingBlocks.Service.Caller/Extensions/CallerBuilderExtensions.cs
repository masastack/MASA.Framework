// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerBuilderExtensions
{
    public static void UseCustomCaller(
        this CallerBuilder callerOptionsBuilder,
        Func<IServiceProvider, IManualCaller> implementationFactory)
    {
        callerOptionsBuilder.Services.Configure<CallerFactoryOptions>(callerOptions =>
        {
            if (callerOptions.Options.Any(relation => relation.Name.Equals(callerOptionsBuilder.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The caller name already exists, please change the name, the repeat name is [{callerOptionsBuilder.Name}]");

            callerOptions.Options.Add(new MasaRelationOptions<IManualCaller>(
                callerOptionsBuilder.Name,
                implementationFactory));
        });
    }
}
