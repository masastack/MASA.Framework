namespace MASA.Contrib.Dispatcher.Events.BenchmarkDotnet.Tests;

class Program
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
