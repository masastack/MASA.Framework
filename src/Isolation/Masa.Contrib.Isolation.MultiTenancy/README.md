[中](README.zh-CN.md) | EN

## Masa.Contrib.Isolation.MultiTenancy

Example：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.MultiTenancy // Multi-tenant isolation On-demand reference
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer
```

1. 配置appsettings.json
``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;",
    "Isolations": [
      {
        "TenantId": "00000000-0000-0000-0000-000000000002",
        "ConnectionString": "server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;"
      },
      {
        "TenantId": "00000000-0000-0000-0000-000000000003",
        "ConnectionString": "server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;"
      }
    ]
  }
}
```

* 1.1 When the current tenant is 00000000-0000-0000-0000-000000000002: database address: server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 When the current tenant is 00000000-0000-0000-0000-000000000003: database address: server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;

2. Using Isolation.UoW.EF
```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.UseMultiTenancy());// Use tenant isolation
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

4. The class corresponding to the isolated table needs to implement IMultiTenant

You can also choose not to implement IMultiTenant when using physical isolation

> The tenant id type can be specified by yourself, the default Guid type
> * For example: Implement IMultiTenant to implement IMultiTenant<int>, UseMultiTenancy() to UseMultiTenancy<int>()

##### Summarize

* How to resolve the tenant in the controller or MinimalAPI?
  * The tenant provides 6 parsers by default, HttpContextItemTenantParserProvider、the execution order is: QueryStringTenantParserProvider, FormTenantParserProvider, RouteTenantParserProvider, HeaderTenantParserProvider, CookieTenantParserProvider (tenant parameter default: __tenant)
* If the resolver fails to resolve the tenant, what is the last database used?
  * If the resolution of the tenant fails, return the DefaultConnection directly
* How to change the default tenant parameter name

```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetTenantKey("tenant").UseMultiTenancy());// Use tenant isolation
});
````
* How to change the parser

```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetTenantParsers(new List<ITenantParserProvider>()
        {
            new QueryStringTenantParserProvider()
        }).UseMultiTenancy());// Use tenant isolation
});
````