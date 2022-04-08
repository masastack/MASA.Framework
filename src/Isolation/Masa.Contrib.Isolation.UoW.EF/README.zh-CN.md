中 | [EN](README.md)

## Masa.Contrib.Isolation.UoW.EF

用例：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.MultiEnvironment // 环境隔离 按需引用
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
        isolationBuilder => isolationBuilder.UseMultiEnvironment().UseMultiTenant(),// 按需选择使用环境或者租户隔离
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

4. 隔离的表对应的类需要实现IMultiTenant或IMultiEnvironment

租户隔离实现IMultiTenant、环境隔离实现IMultiEnvironment

##### 总结
* 解析器目前支持几种？
  * 目前支持两种解析器，一个是[环境解析器](../Masa.Contrib.Isolation.MultiEnvironment/README.zh-CN.md)、一个是[租户解析器](../Masa.Contrib.Isolation.MultiTenant/README.zh-CN.md)
* 解析器如何使用？
  * 通过EventBus发布事件后，EventBus会自动调用解析器中间件，根据隔离性使用情况触发环境、租户解析器进行解析并赋值
  * 针对未使用EventBus的场景，需要在Program.cs中添加`app.UseIsolation();`，在请求发起后会先经过Isolation提供的AspNetCore中间件，并触发环境、租户解析器进行解析并赋值，当请求到达指定的控制器或者Minimal的方法时已经解析完成
* 解析器的作用？
  * 通过解析器获取当前的环境以及租户信息，为隔离提供数据支撑，需要在创建DbContext之前被调用执行