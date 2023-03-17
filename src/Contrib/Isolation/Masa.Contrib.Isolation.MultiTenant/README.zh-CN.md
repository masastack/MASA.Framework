中 | [EN](README.md)

## Masa.Contrib.Isolation.MultiTenant

用例：

``` powershell
Install-Package Masa.Contrib.Isolation.UoW.EFCore //基于EFCore实现Isolation的工作单元，不需要Isolation的请使用Masa.Contrib.Data.UoW.EFCore
Install-Package Masa.Contrib.Isolation.MultiTenant
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

* 1.1 当前租户为00000000-0000-0000-0000-000000000002时：数据库地址：server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 当前租户为00000000-0000-0000-0000-000000000003时：数据库地址：server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.3 其他租户或宿主：数据库地址：server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. 使用Isolation

```csharp
builder.Services.AddIsolation(isolationBuilder =>
{
    isolationBuilder.UseMultiTenant();//使用多租户隔离
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

4. Add data context

```csharp
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
     optionsBuilder.UseSqlServer();//使用SqlServer数据库，或者自己选择其他实现
});
```

5. 隔离的表对应的类需要实现IMultiTenant

采用物理隔离时也可以选择不实现IMultiTenant

### 高级

#### 自定义租户id类型

租户id默认Guid类型，我们可通过以下方式来更改租户id类型

* 方案1

```csharp
builder.Services.AddIsolation(isolationBuilder =>
{
    isolationBuilder.UseMultiTenant(); //使用多租户隔离
}, options => options.MultiTenantType = typeof(int));
```

* 方案2

```csharp
builder.Services.Configure<IsolationOptions>(options => options.MultiTenantType = typeof(int));
```

### 总结

* 控制器或MinimalAPI中租户如何解析？
  * 租户默认提供了7个解析器，执行顺序分别为：HttpContextItemParserProvider、QueryStringParserProvider、FormParserProvider、RouteParserProvider、HeaderParserProvider、CookieParserProvider (租户参数默认：__tenant)
    * CurrentUserEnvironmentParseProvider: 通过从当前登录用户信息中获取租户信息
    * HttpContextItemParserProvider: 通过请求的HttpContext的Items属性获取租户信息
    * QueryStringParserProvider: 通过请求的QueryString获取租户信息
    * https://github.com/masastack?__tenant=1 (租户id为1)
    * FormParserProvider: 通过Form表单获取租户信息
    * RouteParserProvider: 通过路由获取租户信息
    * HeaderParserProvider: 通过请求头获取租户信息
    * CookieParserProvider: 通过Cookie获取租户信息
* 如果解析器解析租户失败，那最后使用的数据库是？
  * 若解析租户失败，则直接返回DefaultConnection
* 如何更改默认租户参数名

``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        isolationBuilder => isolationBuilder.UseMultiTenant("tenant"),// 使用租户隔离
        dbOptions => dbOptions.UseSqlServer());
});
```
* 默认解析器不好用，希望更改默认解析器?

``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        isolationBuilder => isolationBuilder.UseMultiTenant("tenant", new List<ITenantParserProvider>()
        {
            new QueryStringTenantParserProvider() // 只使用QueryStringTenantParserProvider, 其它解析器移除掉
        }),
        dbOptions => dbOptions.UseSqlServer());// 使用租户隔离
});
```

