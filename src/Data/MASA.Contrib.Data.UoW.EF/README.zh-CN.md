中 | [EN](README.md)

## Contracts.EF

用例：

```C#
Install-Package MASA.Contrib.Data.UoW.EF
Install-Package MASA.Contrib.Data.Contracts.EF
```

```C#
builder.Services
    .AddUoW<CustomDbContext>(dbOptions =>
    {
        dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
    })
```

