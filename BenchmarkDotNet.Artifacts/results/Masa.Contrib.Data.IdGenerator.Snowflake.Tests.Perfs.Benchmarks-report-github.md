``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
12th Gen Intel Core i7-12700, 1 CPU, 20 logical and 12 physical cores
.NET SDK=6.0.301
  [Host]     : .NET 6.0.6 (6.0.622.26707), X86 RyuJIT
  Job-SKCHPG : .NET 6.0.6 (6.0.622.26707), X86 RyuJIT
  Job-CBRGIO : .NET 6.0.6 (6.0.622.26707), X86 RyuJIT
  Job-HVWJFS : .NET 6.0.6 (6.0.622.26707), X86 RyuJIT

Runtime=.NET 6.0  RunStrategy=ColdStart  

```
|                                      Method |        Job | IterationCount |        Mean |         Error |         StdDev |     Median |         Min |              Max |  Ratio |   RatioSD |
|-------------------------------------------- |----------- |--------------- |------------:|--------------:|---------------:|-----------:|------------:|-----------------:|-------:|----------:|
|                      SnowflakeByMillisecond | Job-SKCHPG |           1000 |  1,289.9 ns |     809.59 ns |     7,757.3 ns |   600.0 ns | 400.0000 ns |     241,900.0 ns |   1.00 |      0.00 |
|                           SnowflakeBySecond | Job-SKCHPG |           1000 |  2,181.6 ns |     637.30 ns |     6,106.5 ns | 1,500.0 ns | 500.0000 ns |     188,700.0 ns |   2.66 |      2.40 |
| SnowflakeByMillisecondAndEnableMachineClock | Job-SKCHPG |           1000 |  1,090.3 ns |     670.19 ns |     6,421.7 ns |   600.0 ns | 400.0000 ns |     202,800.0 ns |   1.20 |      1.11 |
|      SnowflakeBySecondAndEnableMachineClock | Job-SKCHPG |           1000 |  1,191.9 ns |     788.38 ns |     7,554.1 ns |   700.0 ns | 400.0000 ns |     238,600.0 ns |   1.29 |      1.29 |
|                                             |            |                |             |               |                |            |             |                  |        |           |
|                      SnowflakeByMillisecond | Job-CBRGIO |          10000 |    557.0 ns |      70.56 ns |     2,143.7 ns |   300.0 ns | 100.0000 ns |     206,800.0 ns |   1.00 |      0.00 |
|                           SnowflakeBySecond | Job-CBRGIO |          10000 | 54,185.6 ns | 124,869.91 ns | 3,793,708.0 ns |   300.0 ns | 100.0000 ns | 277,983,700.0 ns | 204.87 | 15,324.25 |
| SnowflakeByMillisecondAndEnableMachineClock | Job-CBRGIO |          10000 |    410.5 ns |      77.45 ns |     2,353.2 ns |   200.0 ns |   0.0000 ns |     214,400.0 ns |   0.82 |      4.36 |
|      SnowflakeBySecondAndEnableMachineClock | Job-CBRGIO |          10000 |    380.3 ns |      79.85 ns |     2,426.0 ns |   100.0 ns |   0.0000 ns |     223,600.0 ns |   0.76 |      4.27 |
|                                             |            |                |             |               |                |            |             |                  |        |           |
|                      SnowflakeByMillisecond | Job-HVWJFS |         100000 |    315.9 ns |       8.18 ns |       786.6 ns |   200.0 ns | 100.0000 ns |     207,600.0 ns |   1.00 |      0.00 |
|                           SnowflakeBySecond | Job-HVWJFS |         100000 | 67,449.6 ns |  46,078.57 ns | 4,428,133.9 ns |   200.0 ns | 100.0000 ns | 298,797,000.0 ns | 288.64 | 21,433.16 |
| SnowflakeByMillisecondAndEnableMachineClock | Job-HVWJFS |         100000 |    145.0 ns |       7.34 ns |       705.2 ns |   100.0 ns |   0.0000 ns |     209,700.0 ns |   0.57 |      0.65 |
|      SnowflakeBySecondAndEnableMachineClock | Job-HVWJFS |         100000 |    145.0 ns |       9.02 ns |       866.5 ns |   100.0 ns |   0.0000 ns |     259,000.0 ns |   0.57 |      0.73 |
