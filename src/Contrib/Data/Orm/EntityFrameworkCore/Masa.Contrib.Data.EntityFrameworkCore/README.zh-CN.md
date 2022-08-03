中 | [EN](README.md)

## Masa.Contrib.Data.EntityFrameworkCore

## 用例:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore
Install-Package Masa.Contrib.Data.Contracts.EF
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

- [SqlServer](../Masa.Contrib.Data.EntityFrameworkCore.SqlServer/README.zh-CN.md)
- [MySql](../Masa.Contrib.Data.EntityFrameworkCore.MySql/README.zh-CN.md)
- [Pomelo.MySql](../Masa.Contrib.Data.EntityFrameworkCore.Pomelo.MySql/README.zh-CN.md)
- [Sqlite](../Masa.Contrib.Data.EntityFrameworkCore.Sqlite/README.zh-CN.md)
- [Cosmos](../Masa.Contrib.Data.EntityFrameworkCore.Cosmos/README.zh-CN.md)
- [InMemory](../Masa.Contrib.Data.EntityFrameworkCore.InMemory/README.zh-CN.md)
- [Oracle](../Masa.Contrib.Data.EntityFrameworkCore.Oracle/README.zh-CN.md)
- [PostgreSql](../Masa.Contrib.Data.EntityFrameworkCore.PostgreSql/README.zh-CN.md)

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