[中](README.zh-CN.md) | EN

## Masa.Contrib.Isolation.UoW.EF

Example：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.Environment // Environmental isolation Quote on demand
Install-Package Masa.Contrib.Isolation.MultiTenant // Multi-tenant isolation On-demand reference
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer
```

1. 配置appsettings.json
``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;",
    "Isolations": [
      {
        "TenantId": "*",// match all tenants
        "Environment": "development",
        "ConnectionString": "server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;",
        "Score": 99 // When multiple environments are matched according to the conditions, the highest one is selected as the link address of the current DbContext according to the descending order of scores. The default Score is 100.
      },
      {
        "TenantId": "00000000-0000-0000-0000-000000000002",
        "Environment": "development",
        "ConnectionString": "server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;"
      }
    ]
  }
}
```

* 1.1 When the current environment is equal to development:
  * When the tenant is equal to 00000000-0000-0000-0000-000000000002, the database address: server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
  * When the tenant is not equal to 00000000-0000-0000-0000-000000000002, the database address: server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;

* 1.2 When the environment is not equal to development:
  * No tenant distinction, database address: server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. 使用Isolation.UoW.EF
``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        isolationBuilder => isolationBuilder.UseMultiEnvironment().UseMultiTenant(),// Select usage environment or tenant isolation as needed
        dbOptions => dbOptions.UseSqlServer());
});
```

3. DbContext needs to inherit IsolationDbContext

``` C#
public class CustomDbContext : IsolationDbContext
{
    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}
```

4. The class corresponding to the isolated table needs to implement IMultiTenant or IMultiEnvironment

Tenant isolation implements IMultiTenant, and environment isolation implements IMultiEnvironment

##### Summarize
* How many kinds of parser are currently supported?
   * Currently two kinds of parsers are supported, one is [Environment Parser](../Masa.Contrib.Isolation.Environment/README.md), the other is [Tenant Parser](../Masa.Contrib.Isolation.MultiTenant/README.md)
* How is the parser used?
   * After publishing events through EventBus, EventBus will automatically call the parser middleware to trigger the environment and tenant parser to parse and assign values according to the isolation usage
   * For scenarios where EventBus is not used, `app.UseIsolation();` needs to be added to Program.cs. After the request is initiated, it will first pass through the AspNetCore middleware provided by Isolation, and trigger the environment and tenant resolvers to parse and assign values. When the request arrives at the specified controller or Minimal method, the parsing is complete
* What does the parser do?
   * Obtain the current environment and tenant information through the parser to provide data support for isolation, which needs to be called and executed before creating DbContext