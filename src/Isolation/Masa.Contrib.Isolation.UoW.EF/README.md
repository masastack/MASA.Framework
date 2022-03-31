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
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.UseEnvironment().UseMultiTenant());// Select usage environment or tenant isolation as needed
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

4. The class corresponding to the isolated table needs to implement IMultiTenant or IEnvironment

Tenant isolation implements IMultiTenant, and environment isolation implements IEnvironment

##### Summarize
* When are tenants and environments resolved?
    * Manipulating DbContext, Repository, etc. in EventHandler can be used directly without any additional operations and no increase in workload. When Event is published, the environment and tenant information will be parsed through the provided parser. If the environment or tenant has been assigned, then will skip parsing
    * To operate DbContext, Repository, etc. directly in the controller or MinimalAPI, you need to add `app.UseIsolation();` in Program.cs, the environment and tenant information will be parsed in the middleware of AspNetCore, if the environment or tenant has been assigned, will skip parsing