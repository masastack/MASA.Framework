中 | [EN](README.md)

## Masa.Contrib.Data.EFCore

## 用例:

```c#
Install-Package Masa.Contrib.Data.EFCore
Install-Package Masa.Contrib.Data.Contracts.EFCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer//这里以SqlServer举例
```

#### 基本用法:

使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter();//启用过滤
    optionsBuilder.DbContextOptionsBuilder.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
});
```

推荐用法:

- [SqlServer](../Masa.Contrib.Data.EFCore.SqlServer/README.zh-CN.md)
- [MySql](../Masa.Contrib.Data.EFCore.MySql/README.zh-CN.md)
- [Pomelo.MySql](../Masa.Contrib.Data.EFCore.Pomelo.MySql/README.zh-CN.md)
- [Sqlite](../Masa.Contrib.Data.EFCore.Sqlite/README.zh-CN.md)
- [Cosmos](../Masa.Contrib.Data.EFCore.Cosmos/README.zh-CN.md)
- [InMemory](../Masa.Contrib.Data.EFCore.InMemory/README.zh-CN.md)
- [Oracle](../Masa.Contrib.Data.EFCore.Oracle/README.zh-CN.md)
- [PostgreSql](../Masa.Contrib.Data.EFCore.PostgreSql/README.zh-CN.md)

#### 数据过滤器

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