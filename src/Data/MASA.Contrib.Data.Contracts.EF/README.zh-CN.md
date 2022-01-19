中 | [EN](README.md)

## Contracts.EF

用例：

```C#
Install-Package MASA.Contrib.Data.UoW.EF
Install-Package MASA.Contrib.Data.Contracts.EF
```

```C#
builder.Services.AddEventBus(options => {
    options.UseUoW<CustomDbContext>(dbOptions =>
    {
        dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
        dbOptions.UseSoftDelete(builder.Services);//启动软删除
    });
});
```

> 当实体继承ISoftware，且被删除时，将删除状态改为修改状态，并配合自定义Remove操作，实现软删除
> 支持查询的时候不查询被标记软删除的数据
> 与EventBus结合使用时，做到了第一次CUD后开启事务，当整个Handler出现异常后支持事务回滚

> 常见问题：

- 问题1：使用UseSoftDelete后出现提交保存不上的问题

      使用Uow后，默认在进行Add、Modified、Deleted后会启用事务
      需要提交事务之后才能正常保存
      如果使用EventBus则会自动提交事务