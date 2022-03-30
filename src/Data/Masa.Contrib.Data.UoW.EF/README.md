[中](README.zh-CN.md) | EN

## UoW.EF

Example：

```C#
Install-Package Masa.Contrib.Data.UoW.EF
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer
```

```C#
builder.Services.AddEventBus(options => {
    options.UseUoW<CustomDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"));
});
```