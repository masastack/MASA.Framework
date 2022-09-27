[中](README.zh-CN.md) | EN

## Masa.Utils.Data.Prometheus

[Prometheus Http Api](https://www.prometheus.io/docs/prometheus/latest/querying/api/) Client Library

## Install:
```c#
Install-Package Masa.Utils.Data.Prometheus
```

Example:

1. Inject

```` C#
builder.Services.AddPrometheusClient("http://127.0.0.1:9090");
````

2. Query Example

```C#
public class SampleService
{

    private IMasaPrometheusClient _client;

    public SampleService(IMasaPrometheusClient client)
    {
        _client=client;
    }

    public async Task QueryAsync()
    {
        var query= new QueryRequest {
            Query = "up", //metric name
            Time = "2022-06-01T09:00:00.000Z" //standard time format or unix timestamp, such as: 1654045200 or 1654045200.000
        };
        var result = await _client.QueryAsync(query);
        if(result.Status == ResultStatuses.Success)
        {
            switch(result.Data.ResultType)
            {
                case ResultTypes.Vector:
                    {
                        var data=result.Data.Result as QueryResultInstantVectorResponse[];
                        ...
                    }
                    break;
                case ResultTypes.Matrix:
                    {
                        var data=result.Data.Result as QueryResultMatrixRangeResponse[];
                        ...
                    }
                    break;
                default:
                    {
                        var timeSpan=(double)result.Data.Result[0];
                        var value=(string)result.Data.Result[1];
                    }
                    break;
            }
        }
    }
}
```

### Current suports:

- [query](https://www.prometheus.io/docs/prometheus/latest/querying/api/#instant-queries)
- [query_range](https://www.prometheus.io/docs/prometheus/latest/querying/api/#range-queries)
- [series](https://www.prometheus.io/docs/prometheus/latest/querying/api/#finding-series-by-label-matchers)
- [lables](https://www.prometheus.io/docs/prometheus/latest/querying/api/#getting-label-names)
- [lable value](https://www.prometheus.io/docs/prometheus/latest/querying/api/#querying-label-values)
- [exemplars](https://www.prometheus.io/docs/prometheus/latest/querying/api/#querying-exemplars)
