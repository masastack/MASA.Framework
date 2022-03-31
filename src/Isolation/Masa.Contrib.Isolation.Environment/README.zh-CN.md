中 | [EN](README.md)

## Masa.Contrib.Isolation.Environment

用例：

```C#
Install-Package Masa.Contrib.Isolation.UoW.EF
Install-Package Masa.Contrib.Isolation.Environment
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer
```

1. 配置appsettings.json
``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;",
    "Isolations": [
      {
        "Environment": "development",
        "ConnectionString": "server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;"
      },
      {
        "Environment": "staging",
        "ConnectionString": "server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;"
      }
    ]
  }
}
```

* 1.1 当前环境是development时：数据库地址：server=localhost,1674;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.2 当前环境是staging时：数据库地址：server=localhost,1672;uid=sa;pwd=P@ssw0rd;database=identity;
* 1.3 当前环境是其他环境时：数据库地址：server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;

2. 使用Isolation.UoW.EF
``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.UseEnvironment());
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

4. 隔离的表对应的类需要实现IEnvironment

采用物理隔离时也可以选择不实现IEnvironment

##### 总结

* 控制器或MinimalAPI中环境如何解析？
    * 环境默认提供了1个解析器，执行顺序为：EnvironmentVariablesParserProvider (在系统环境变量中获取，环境参数默认：ASPNETCORE_ENVIRONMENT)
* 如果解析器解析环境失败，那最后使用的数据库是?
    * 若解析环境失败，则直接返回DefaultConnection
* 如何更改默认环境参数名

``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetEnvironmentKey("env").UseEnvironment());// 使用环境隔离
});
```
* 如何更改解析器

``` C#
builder.Services.AddEventBus(eventBusBuilder =>
{
    eventBusBuilder.UseIsolationUoW<CustomDbContext>(
        dbOptions => dbOptions.UseSqlServer(),
        isolationBuilder => isolationBuilder.SetEnvironmentParsers(new List<IEnvironmentParserProvider>()
        {
            new EnvironmentVariablesParserProvider()
        }).UseEnvironment());// 使用环境隔离
});
```