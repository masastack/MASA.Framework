[中](README.zh-CN.md) | EN

## Masa.Contrib.Isolation.Environment

Example：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.Environment
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
        isolationBuilder => isolationBuilder.UseEnvironment());
});
````

3. DbContext needs to inherit IsolationDbContext

```` C#
public class CustomDbContext : IsolationDbContext
{
    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}
````

4. The class corresponding to the isolated table needs to implement IMultiEnvironment

You can also choose not to implement IMultiEnvironment when using physical isolation

##### Summarize

* How is the environment resolved in the controller or MinimalAPI?
    * The environment provides one parser by default, and the execution order is: EnvironmentVariablesParserProvider (gets the parameters in the system environment variables, the parameters of the default environment isolation: ASPNETCORE_ENVIRONMENT)
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
            new EnvironmentVariablesParserProvider() //By default, environment information in environment isolation is obtained from system environment variables
        }).UseEnvironment());// Use environment isolation
});
````