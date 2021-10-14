## Contracts.EF

用例：

```C#
Install-Package MASA.Contribs.Data.Contracts.EF
```

```C#
builder.Services
    .AddUoW<CustomDbContext>(dbOptions =>
    {
        dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
        dbOptions.UseSoftDelete(builder.Services);//启动软删除
    })
```

> 当实体继承ISoftware，且被删除时，将删除状态改为修改状态，并配合自定义Remove操作，实现软删除
> 支持查询的时候不查询被标记软删除的数据
> 与EventBus结合使用时，做到了第一次CUD后开启事务，当整个Handler出现异常后支持事务回滚
