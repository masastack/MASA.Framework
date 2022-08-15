``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
12th Gen Intel Core i7-12700, 1 CPU, 20 logical and 12 physical cores
.NET SDK=6.0.301
  [Host]     : .NET 6.0.6 (6.0.622.26707), X86 RyuJIT
  Job-HVEJYK : .NET 6.0.6 (6.0.622.26707), X86 RyuJIT

Runtime=.NET 6.0  IterationCount=100  RunStrategy=ColdStart  

```
|                         Method |      Mean |     Error |      StdDev |   Median |      Min |         Max | Ratio | RatioSD |
|------------------------------- |----------:|----------:|------------:|---------:|---------:|------------:|------:|--------:|
|             SendCouponByDirect |  16.14 μs |  35.75 μs |   105.41 μs | 4.450 μs | 1.600 μs |  1,058.6 μs |  1.00 |    0.00 |
|           SendCouponByEventBus | 256.63 μs | 812.48 μs | 2,395.63 μs | 6.000 μs | 3.800 μs | 23,965.0 μs |  4.52 |   13.37 |
| AddShoppingCartByEventBusAsync | 108.53 μs | 294.35 μs |   867.89 μs | 5.600 μs | 2.600 μs |  8,653.4 μs |  8.04 |   35.47 |
|  AddShoppingCartByMediatRAsync | 115.21 μs | 334.10 μs |   985.09 μs | 3.450 μs | 2.200 μs |  9,834.5 μs |  3.68 |   12.66 |
