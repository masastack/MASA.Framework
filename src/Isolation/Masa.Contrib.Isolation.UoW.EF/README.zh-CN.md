中 | [EN](README.md)

## Masa.Contrib.Isolation.UoW.EF

用例：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.Environment // 环境隔离 按需引用
Install-Package Masa.Contrib.Isolation.MultiTenant // 多租户隔离 按需引用
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer
```

1. 配置appsettings.json
``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;",
    "Isolations": [
      {
        "TenantId": "*",//匹配所有租户
        "Environment": "development",
        "ConnectionString": "server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;",
        "Score": 99 //当根据条件匹配到多个环境时，根据分值降序选择其中最高的作为当前DbContext的链接地址，Score默认为100
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

* 1.1 当前环境等于development时：
  * 当租户等于00000000-0000-0000-0000-000000000002时，数据库地址：server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
  * 当租户不等于00000000-0000-0000-0000-000000000002时，数据库地址：server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;

* 1.2 当环境不等于development时：
  * 不区分租户，数据库地址：server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. 使用Isolation.UoW.EF
``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.UseEnvironment().UseMultiTenant());// 按需选择使用环境或者租户隔离
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

4. 隔离的表对应的类需要实现IMultiTenant或IMultiEnvironment

租户隔离实现IMultiTenant、环境隔离实现IMultiEnvironment

##### 总结
* 租户与环境什么时候被解析？
  * 在Event被Publish会通过提供的解析器解析环境、租户信息，如果环境或者租户已经被赋值，则会跳过解析
  * 不使用EventBus，直接在控制器或MinimalAPI中操作DbContext、Repository等需要在Program.cs中添加`app.UseIsolation();`，在AspNetCore的中间件中会解析环境、租户信息，如果环境或者租户已经被赋值，则会跳过解析
