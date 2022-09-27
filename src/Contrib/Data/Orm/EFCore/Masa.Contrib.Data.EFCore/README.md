[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore

Example:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.SqlServer // SqlServer is used as an example here, you can also choose other implementations by yourself
Install-Package Masa.Contrib.Data.Contracts.EFCore //Use the data filtering and soft delete capabilities provided by the protocol, if you don't need it, you can not refer to it
```

### Get Started

1. Configure `appsettings.json`

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"
  }
}
```

2. Register `MasaDbContext`

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter();// Enable filtering, provided by Masa.Contrib.Data.Contracts.EFCore
    optionsBuilder.UseSqlServer();//Use SqlServer database, you can also choose other implementations
});
```

Recommended usage:

- [SqlServer](../Masa.Contrib.Data.EFCore.SqlServer/README.zh-CN.md)
- [Pomelo.MySql](../Masa.Contrib.Data.EFCore.Pomelo.MySql/README.zh-CN.md): If you are using mysql, it is recommended
- [MySql](../Masa.Contrib.Data.EFCore.MySql/README.zh-CN.md)
- [Sqlite](../Masa.Contrib.Data.EFCore.Sqlite/README.zh-CN.md)
- [Cosmos](../Masa.Contrib.Data.EFCore.Cosmos/README.zh-CN.md)
- [InMemory](../Masa.Contrib.Data.EFCore.InMemory/README.zh-CN.md)
- [Oracle](../Masa.Contrib.Data.EFCore.Oracle/README.zh-CN.md)
- [PostgreSql](../Masa.Contrib.Data.EFCore.PostgreSql/README.zh-CN.md)

### Data filter

``` C#
public async Task<string> GetAllAsync([FromServices] IRepository<Users> repository, [FromServices] IDataFilter dataFilter)
{
    // Temporarily disable soft delete filtering
    using (dataFilter.Disable<ISoftDelete>())
    {
        var list = (await repository.GetListAsync()).ToList();
        return System.Text.Json.JsonSerializer.Serialize(list);
    }
}
```