// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Service.Caller.HttpClient")]
[assembly: InternalsVisibleTo("Masa.Contrib.Service.Caller.DaprClient")]

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Service.Caller;

internal static class CallerOptionsBuilderExtensions
{
    public static void AddCallerRelation(
        this CallerOptionsBuilder callerOptionsBuilder,
        Func<IServiceProvider, ICallerDisposeWrapper> implementationFactory)
    {
        callerOptionsBuilder.Services.Configure<CallerFactoryOptions>(callerOptions =>
        {
            if (callerOptions.Options.Any(relation => relation.Name.Equals(callerOptionsBuilder.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The caller name already exists, please change the name, the repeat name is [{callerOptionsBuilder.Name}]");

            callerOptions.Options.Add(new CallerRelationOptions(
                callerOptionsBuilder.Name,
                implementationFactory,
                callerOptionsBuilder.Lifetime));
        });
    }
}
