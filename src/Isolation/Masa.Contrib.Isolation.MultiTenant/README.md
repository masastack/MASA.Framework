[中](README.zh-CN.md) | EN

## Masa.Contrib.Isolation.MultiTenant

Example：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
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
* 1.3 Other tenants: database address: server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. Using Isolation.UoW.EF
```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.UseMultiTenant());// Use tenant isolation
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

4. The class corresponding to the isolated table needs to implement IMultiTenant

You can also choose not to implement IMultiTenant when using physical isolation

> The tenant id type can be specified by yourself, the default Guid type
> * For example: Implement IMultiTenant to implement IMultiTenant<int>, UseMultiTenant() to UseMultiTenant<int>()

##### Summarize

* How to resolve the tenant in the controller or MinimalAPI?
  * The tenant provides 6 parsers by default, the execution order is: HttpContextItemTenantParserProvider, QueryStringTenantParserProvider, FormTenantParserProvider, RouteTenantParserProvider, HeaderTenantParserProvider, CookieTenantParserProvider (tenant parameter default: __tenant)
    * HttpContextItemTenantParserProvider: Obtain tenant information through the Items property of the requested HttpContext
    * QueryStringTenantParserProvider: Get tenant information through the requested QueryString
      * https://github.com/masastack?__tenant=1 (tenant id is 1)
    * FormTenantParserProvider: Get tenant information through the Form form
    * RouteTenantParserProvider: Get tenant information through routing
    * HeaderTenantParserProvider: Get tenant information through request headers
    * CookieTenantParserProvider: Get tenant information through cookies
* If the resolver fails to resolve the tenant, what is the last database used?
  * If the resolution of the tenant fails, return the DefaultConnection directly
* How to change the default tenant parameter name

```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetTenantKey("tenant").UseMultiTenant());// Use tenant isolation
});
````
* The default parser is not easy to use, want to change the default parser?

```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetTenantParsers(new List<ITenantParserProvider>()
        {
            new QueryStringTenantParserProvider() // only use QueryStringTenantParserProvider, other parsers are removed
        }).UseMultiTenant());// Use tenant isolation
});
````
````