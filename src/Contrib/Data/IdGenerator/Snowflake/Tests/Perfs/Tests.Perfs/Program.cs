// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Tests.Perfs;

static class Program
{
    static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .AddValidator(ExecutionValidator.FailOnError)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);
        BenchmarkRunner.Run<Benchmarks>(config);
        Console.ReadLine();
    }
}
