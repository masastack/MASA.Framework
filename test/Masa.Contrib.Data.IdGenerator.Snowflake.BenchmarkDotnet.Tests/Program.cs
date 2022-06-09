// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.BenchmarkDotnet.Tests;

class Program
{
    static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .AddValidator(ExecutionValidator.FailOnError)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);
        // BenchmarkRunner.Run<Benchmarks>(config);
        // BenchmarkRunner.Run<SecondBenchmarks>(config);
        BenchmarkRunner.Run<MachineClockBenchmarks>(config);
        // BenchmarkRunner.Run<DistributedBenchmarks>(config);
        Console.ReadLine();
    }
}
