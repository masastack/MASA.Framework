namespace MASA.Contrib.Dispatcher.InMemory.BenchmarkDotnetTest;

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
