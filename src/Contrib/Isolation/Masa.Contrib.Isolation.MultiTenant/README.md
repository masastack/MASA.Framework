[中](README.zh-CN.md) | EN

## Masa.Contrib.Isolation.MultiTenant

Example：

``` powershell
Install-Package Masa.Contrib.Isolation.UoW.EFCore //Isolation work unit based on EFCore, please use Masa.Contrib.Data.UoW.EFCore if Isolation is not required
Install-Package Masa.Contrib.Isolation.MultiTenant
Install-Package Masa.Contrib.Data.EFCore.SqlServer //Based on EFCore and SqlServer database usage
```

### Get Started

1. Configure `appsettings.json`

``` appsettings.json
{
    "ConnectionStrings":{
        "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;"
    },
    "Isolation":{
        "ConnectionStrings":[
            {
                "TenantId":"00000000-0000-0000-0000-000000000002",
                "Data":{
                    "ConnectionString": "server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;"
                }
            },
            {
                "TenantId":"00000000-0000-0000-0000-000000000003",
                "Data":{
                    "ConnectionString": "server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;"
                }
            }
        ]
    }
}
```

* 1.1 When the current tenant is 00000000-0000-0000-0000-000000000002: database address: server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 When the current tenant is 00000000-0000-0000-0000-000000000003: database address: server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.3 Other tenants or hosts: database address: server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. Using Isolation

```csharp
builder.Services.AddIsolation(isolationBuilder =>
{
    isolationBuilder.UseMultiTenant();//Use Tenant Isolation
});
```

3. DbContext needs to inherit `MasaDbContext<>`

```csharp
public class CustomDbContext : MasaDbContext<CustomDbContext>
{
    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}
```

4. Add data context

```csharp
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
     optionsBuilder.UseSqlServer();//Use the SqlServer database, or choose other implementations by yourself
});
```

5. The class corresponding to the isolated table needs to implement IMultiTenant

### Advanced

The default type of tenant id is Guid, we can change the type of tenant id in the following ways

* plan 1

```csharp
builder.Services.AddIsolation(isolationBuilder =>
{
     isolationBuilder.UseMultiTenant(); //Use multi-tenant isolation
}, options => options. MultiTenantType = typeof(int));
```

* plan 2

```csharp
builder.Services.Configure<IsolationOptions>(options => options.MultiTenantType = typeof(int));
```

### Summarize

* How to resolve the tenant in the controller or MinimalAPI?
  * The tenant provides 7 parsers by default, the execution order is: HttpContextItemParserProvider、QueryStringParserProvider、FormParserProvider、RouteParserProvider、HeaderParserProvider、CookieParserProvider (tenant parameter default: __tenant)
    * CurrentUserEnvironmentParseProvider: By getting the tenant information from the currently logged in user information
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

``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        isolationBuilder => isolationBuilder.UseMultiTenant("tenant"),
        dbOptions => dbOptions.UseSqlServer());// Use tenant isolation
});
```
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
