中 | [EN](README.md)

## Masa.Contrib.Isolation.MultiTenant

用例：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.MultiTenant
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer
```

1. 配置`appsettings.json`
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

* 1.1 当前租户为00000000-0000-0000-0000-000000000002时：数据库地址：server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 当前租户为00000000-0000-0000-0000-000000000003时：数据库地址：server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.3 其他租户或宿主：数据库地址：server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. 使用Isolation.UoW.EF
``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        isolationBuilder => isolationBuilder.UseMultiTenant(),// 使用租户隔离
        dbOptions => dbOptions.UseSqlServer());
});
```

3. DbContext需要继承IsolationDbContext

``` C#
public class CustomDbContext : IsolationDbContext
{
    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}
```

4. 隔离的表对应的类需要实现IMultiTenant

采用物理隔离时也可以选择不实现IMultiTenant

> 租户id类型支持自行指定，默认Guid类型
> * 如：实现IMultiTenant改为实现IMultiTenant<int>，UseIsolationUoW<CustomDbContext>()改为UseIsolationUoW<CustomDbContext, int>()

##### 总结

* 控制器或MinimalAPI中租户如何解析？
  * 租户默认提供了6个解析器，执行顺序分别为：HttpContextItemParserProvider、QueryStringParserProvider、FormParserProvider、RouteParserProvider、HeaderParserProvider、CookieParserProvider (租户参数默认：__tenant)
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

