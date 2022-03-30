中 | [EN](README.md)

## Masa.Contrib.Isolation.MultiTenancy

用例：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.MultiTenancy // 多租户隔离 按需引用
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

* 1.1 当前租户为00000000-0000-0000-0000-000000000002时：数据库地址：server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 当前租户为00000000-0000-0000-0000-000000000003时：数据库地址：server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;

2. 使用Isolation.UoW.EF
``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.UseMultiTenancy());// 使用租户隔离
});
```

3. DbContext需要继承IsolationDbContext

``` C#
public class CustomDbContext : MasaDbContext
{
    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}
```

4. 隔离的表对应的类需要实现IMultiTenant

采用物理隔离时也可以选择不实现IMultiTenant

> 租户id类型支持自行指定，默认Guid类型
> * 如：实现IMultiTenant改为实现IMultiTenant<int>，UseMultiTenancy()改为UseMultiTenancy<int>()

##### 总结

* 控制器或MinimalAPI中租户如何解析？
  * 租户默认提供了6个解析器，执行顺序分别为：HttpContextItemTenantParserProvider、QueryStringTenantParserProvider、FormTenantParserProvider、RouteTenantParserProvider、HeaderTenantParserProvider、CookieTenantParserProvider (租户参数默认：__tenant)
* 如果解析器解析租户失败，那最后使用的数据库是？
  * 若解析租户失败，则直接返回DefaultConnection
* 如何更改默认租户参数名

``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetTenantKey("tenant").UseMultiTenancy());// 使用租户隔离
});
```
* 如何更改解析器

``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetTenantParsers(new List<ITenantParserProvider>()
        {
            new QueryStringTenantParserProvider()
        }).UseMultiTenancy());// 使用租户隔离
});
```