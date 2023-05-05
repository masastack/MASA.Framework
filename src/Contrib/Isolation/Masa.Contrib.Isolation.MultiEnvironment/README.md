[中](README.zh-CN.md) | EN

## Masa.Contrib.Isolation.MultiEnvironment

Example：

``` powershell
Install-Package Masa.Contrib.Isolation.MultiEnvironment
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
                "Environment":"development",
                "Data":{
                    "ConnectionString": "server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;"
                }
            },
            {
                "Environment":"staging",
                "Data":{
                    "ConnectionString": "server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;"
                }
            }
        ]
    }
}
```
* 1.1 When the current environment is development: database address: server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 When the current environment is staging: database address: server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.3 When the current environment is another environment: database address: server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. Using Isolation

```csharp
builder.Services.AddIsolation(isolationBuilder =>
{
    isolationBuilder.UseMultiEnvironment(),//use environment isolation
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
    optionsBuilder.UseSqlServer();//Use SqlServer database, you can also choose other implementations
});
```

5The class corresponding to the isolated table needs to implement IMultiEnvironment

You can also choose not to implement IMultiEnvironment when using physical isolation

### Summarize

* How is the environment resolved in the controller or MinimalAPI?
    * The environment provides 9 parsers by default, and the execution order is: HttpContextItemParserProvider, QueryStringParserProvider, FormParserProvider, RouteParserProvider, HeaderParserProvider, CookieParserProvider, EnvironmentVariablesParserProvider (to obtain the parameters in the system environment variables, the parameter values ​​used in the default environment isolation: global environment parameter variables (use global Default environment parameter variable in configuration) > **ASPNETCORE_ENVIRONMENT** (used if `MasaConfiguration` is not used)
        * CurrentUserEnvironmentParseProvider: By getting the environment information from the currently logged in user information
        * HttpContextItemParserProvider: Get environment information through the Items property of the requested HttpContext
        * QueryStringParserProvider: Get environment information through the requested QueryString
            * https://github.com/masastack?ASPNETCORE_ENVIRONMENT=development (environment information is development)
        * FormParserProvider: Get environment information through Form form
        * RouteParserProvider: Get environment information through routing
        * HeaderParserProvider: Get environment information through request headers
        * CookieParserProvider: Get environmental information through cookies
        * MasaAppConfigureParserProvider: Get environment information through global configuration
        * EnvironmentVariablesParserProvider: Get environment information through system environment variables
* If the parser fails to resolve the environment, what is the last database used?
    * If the parsing environment fails, return DefaultConnection directly
* How to change the default environment parameter name

``` C#
builder.Services.AddIsolation(isolationBuilder =>
{
    isolationBuilder.UseMultiEnvironment("env"),//use environment isolation
});
```
* How to change the parser

``` C#
builder.Services.AddIsolation(isolationBuilder =>
{
     isolationBuilder.UseMultiEnvironment("env", new List<IEnvironmentParserProvider>()
     {
         new EnvironmentVariablesParserProvider() //Get the environment information in the environment isolation from the system environment variables by default
     });
});
```