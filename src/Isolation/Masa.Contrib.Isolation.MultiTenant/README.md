[中](README.zh-CN.md) | EN

## Masa.Contrib.Isolation.MultiTenant

Example：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.MultiTenant
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer
```

1. Configure `appsettings.json`
``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;"
  },
  "IsolationConnectionStrings": [
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
```

* 1.1 When the current tenant is 00000000-0000-0000-0000-000000000002: database address: server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 When the current tenant is 00000000-0000-0000-0000-000000000003: database address: server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.3 Other tenants or hosts: database address: server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. Using Isolation.UoW.EF
```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        isolationBuilder => isolationBuilder.UseMultiTenant(),// Use tenant isolation
        dbOptions => dbOptions.UseSqlServer());
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
> * For example: Implement IMultiTenant to implement IMultiTenant<int>, UseIsolationUoW<CustomDbContext>() to UseIsolationUoW<CustomDbContext, int>()

##### Summarize

* How to resolve the tenant in the controller or MinimalAPI?
  * The tenant provides 6 parsers by default, the execution order is: HttpContextItemParserProvider、QueryStringParserProvider、FormParserProvider、RouteParserProvider、HeaderParserProvider、CookieParserProvider (tenant parameter default: __tenant)
    * HttpContextItemParserProvider: Obtain tenant information through the Items property of the requested HttpContext
    * QueryStringParserProvider: Get tenant information through the requested QueryString
      * https://github.com/masastack?__tenant=1 (tenant id is 1)
    * FormParserProvider: Get tenant information through the Form form
    * RouteParserProvider: Get tenant information through routing
    * HeaderParserProvider: Get tenant information through request headers
    * CookieParserProvider: Get tenant information through cookies
* If the resolver fails to resolve the tenant, what is the last database used?
  * If the resolution of the tenant fails, return the DefaultConnection directly
* How to change the default tenant parameter name

```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        isolationBuilder => isolationBuilder.UseMultiTenant("tenant"),
        dbOptions => dbOptions.UseSqlServer());// Use tenant isolation
});
````
* The default parser is not easy to use, want to change the default parser?

```` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        isolationBuilder => isolationBuilder.UseMultiTenant("tenant", new List<ITenantParserProvider>()
        {
            new QueryStringTenantParserProvider() // only use QueryStringTenantParserProvider, other parsers are removed
        }),
        dbOptions => dbOptions.UseSqlServer());// Use tenant isolation
});
````
