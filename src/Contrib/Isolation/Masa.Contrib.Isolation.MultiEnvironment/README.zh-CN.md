中 | [EN](README.md)

## Masa.Contrib.Isolation.MultiEnvironment

用例：

``` powershell
Install-Package Masa.Contrib.Isolation.MultiEnvironment
Install-Package Masa.Contrib.Data.EFCore.SqlServer //基于EFCore以及SqlServer数据库使用
```

### 入门

1. 配置`appsettings.json`

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

* 1.1 当前环境是development时：数据库地址：server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 当前环境是staging时：数据库地址：server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.3 当前环境是其他环境时：数据库地址：server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. 使用Isolation

```csharp
builder.Services.AddIsolation(isolationBuilder =>
{
    isolationBuilder.UseMultiEnvironment(),//使用环境隔离
});
```

3. DbContext需要继承`MasaDbContext<>`

```csharp
public class CustomDbContext : MasaDbContext<CustomDbContext>
{
    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}
```

4. 添加数据上下文

```csharp
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlServer();//使用SqlServer数据库，也可自行选择其它实现
});
```

5. 隔离的表对应的类需要实现IMultiEnvironment

采用物理隔离时也可以选择不实现IMultiEnvironment

### 总结

* 控制器或MinimalAPI中环境如何解析？
    * 环境默认提供了9个解析器，执行顺序为：HttpContextItemParserProvider、QueryStringParserProvider、FormParserProvider、RouteParserProvider、HeaderParserProvider、CookieParserProvider、EnvironmentVariablesParserProvider (获取系统环境变量中的参数，默认环境隔离使用的参数值: 全局环境参数变量 (使用全局配置中默认环境参数变量) > **ASPNETCORE_ENVIRONMENT** (如果未使用`MasaConfiguration`则使用)
      * CurrentUserEnvironmentParseProvider: 通过从当前登录用户信息中获取环境信息
      * HttpContextItemParserProvider: 通过请求的HttpContext的Items属性获取环境信息
      * QueryStringParserProvider: 通过请求的QueryString获取环境信息
          * https://github.com/masastack?ASPNETCORE_ENVIRONMENT=development (环境信息是development)
      * FormParserProvider: 通过Form表单获取环境信息
      * RouteParserProvider: 通过路由获取环境信息
      * HeaderParserProvider: 通过请求头获取环境信息
      * CookieParserProvider: 通过Cookie获取环境信息
      * MasaAppConfigureParserProvider: 通过全局配置获取环境信息
      * EnvironmentVariablesParserProvider: 通过系统环境变量获取环境信息
* 如果解析器解析环境失败，那最后使用的数据库是?
    * 若解析环境失败，则直接返回DefaultConnection
* 如何更改默认环境参数名

``` C#
builder.Services.AddIsolation(isolationBuilder =>
{
    isolationBuilder.UseMultiEnvironment("env"),//使用环境隔离
});
```
* 如何更改解析器

``` C#
builder.Services.AddIsolation(isolationBuilder =>
{
    isolationBuilder.UseMultiEnvironment("env", new List<IEnvironmentParserProvider>()
    {
        new EnvironmentVariablesParserProvider() //默认从系统环境变量中获取环境隔离中的环境信息
    });
});
```