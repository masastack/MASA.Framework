中 | [EN](README.md)

## Masa.Contrib.Data.EFCore

用例:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.SqlServer //这里以SqlServer举例，也可自行选择其它实现
Install-Package Masa.Contrib.Data.Contracts.EFCore //使用规约提供的数据过滤、软删除能力，如果不需要可不引用
```

### 入门

1. 配置`appsettings.json`

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"
  }
}
```

2. 注册`MasaDbContext`

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter();//启用过滤，由 Masa.Contrib.Data.Contracts.EFCore 提供
    optionsBuilder.UseSqlServer();//使用SqlServer数据库，也可自行选择其它实现
});
```

推荐用法:

- [SqlServer](../Masa.Contrib.Data.EFCore.SqlServer/README.zh-CN.md)
- [Pomelo.MySql](../Masa.Contrib.Data.EFCore.Pomelo.MySql/README.zh-CN.md)：如果您使用的是mysql，建议使用
- [MySql](../Masa.Contrib.Data.EFCore.MySql/README.zh-CN.md)
- [Sqlite](../Masa.Contrib.Data.EFCore.Sqlite/README.zh-CN.md)
- [Cosmos](../Masa.Contrib.Data.EFCore.Cosmos/README.zh-CN.md)
- [InMemory](../Masa.Contrib.Data.EFCore.InMemory/README.zh-CN.md)
- [Oracle](../Masa.Contrib.Data.EFCore.Oracle/README.zh-CN.md)
- [PostgreSql](../Masa.Contrib.Data.EFCore.PostgreSql/README.zh-CN.md)

### 数据过滤器

``` C#
public async Task<string> GetAllAsync([FromServices] IRepository<Users> repository, [FromServices] IDataFilter dataFilter)
{
    // 临时禁用软删除过滤
    using (dataFilter.Disable<ISoftDelete>())
    {
        var list = (await repository.GetListAsync()).ToList();
        return System.Text.Json.JsonSerializer.Serialize(list);
    }
}
```