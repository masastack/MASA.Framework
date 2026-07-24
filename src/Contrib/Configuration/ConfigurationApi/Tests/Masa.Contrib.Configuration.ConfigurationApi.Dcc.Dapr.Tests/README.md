# Dapr ConfigurationApi Client Tests

This project validates `DaprConfigurationApiClient` read methods against a real Dapr + Redis runtime.

## Covered methods

- `GetRawAsync(environment, cluster, appId, configObject)`
- `GetAsync<T>(environment, cluster, appId, configObject)`
- `GetDynamicAsync(environment, cluster, appId, configObject)`

## Runtime dependencies

- Redis (default `127.0.0.1:6379`)
- Dapr sidecar with a Redis configuration component (default HTTP endpoint `http://127.0.0.1:3500`)

The tests seed Redis keys before read operations, then clean up keys after test completion.

## Quick start

1. Start Redis.
2. Start Dapr sidecar and load `components/configuration.yaml`.
3. Run tests:

```powershell
dotnet test .\Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Tests.csproj -f net8.0
```

Optional environment variables:

- `DAPR_HTTP_ENDPOINT` (default `http://127.0.0.1:3500`)
- `DAPR_TEST_REDIS_CONNECTION` (default `127.0.0.1:6379`)
- `DAPR_TEST_CONFIGURATION_STORE` (default `configuration`)
