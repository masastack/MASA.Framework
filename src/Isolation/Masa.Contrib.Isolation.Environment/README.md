[中](README.zh-CN.md) | EN

## Masa.Contrib.Isolation.Environment

Example：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.Environment // Environmental isolation Quote on demand
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer
```

1. 配置appsettings.json
``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;",
    "Isolations": [
      {
        "Environment": "development",
        "ConnectionString": "server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;"
      },
      {
        "Environment": "staging",
        "ConnectionString": "server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;"
      }
    ]
  }
}
```
* 1.1 When the current environment is development: database address: server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 When the current environment is staging: database address: server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.3 When the current environment is another environment: database address: server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. Using Isolation.UoW.EF
```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.UseEnvironment());// Use environment isolation
});
````

3. DbContext needs to inherit IsolationDbContext

```` C#
public class CustomDbContext : MasaDbContext
{
    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}
````

4. The class corresponding to the isolated table needs to implement IEnvironment

You can also choose not to implement IMultiTenant when using physical isolation

##### Summarize

* How is the environment resolved in the controller or MinimalAPI?
    * The environment provides 1 parser by default, and the execution order is: EnvironmentVariablesParserProvider (obtained in the system environment variable, the default environment parameter: ASPNETCORE_ENVIRONMENT)
* If the parser fails to resolve the environment, what is the last database used?
    * If the parsing environment fails, return DefaultConnection directly
* How to change the default environment parameter name

```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetEnvironmentKey("env").UseEnvironment());// Use environment isolation
});
````
* How to change the parser

```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetEnvironmentParsers(new List<IEnvironmentParserProvider>()
        {
            new EnvironmentVariablesParserProvider()
        }).UseEnvironment());// Use environment isolation
});
````