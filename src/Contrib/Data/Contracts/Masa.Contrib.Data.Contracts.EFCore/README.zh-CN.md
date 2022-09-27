中 | [EN](README.md)

## Masa.Contrib.Data.Contracts.EFCore

提供的数据过滤(用于禁用租户、环境或软删除)、软删除能力

用例:

``` powershelll
Install-Package Masa.Contrib.Data.EFCore.Sqlite //以Sqlite数据库为例
Install-Package Masa.Contrib.Data.Contracts.EFCore //使用规约提供的数据过滤、软删除能力
```

1. 配置appsettings.json

``` appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=test.db;"
  }
}
```

2. 使用MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter();//启用过滤
    optionsBuilder.UseSqlite();//使用Sqlite数据库
});
```

3. 使用数据过滤

``` C#
public async Task<string> GetAllAsync([FromServices] IRepository<Users> repository, [FromServices] IDataFilter dataFilter)
{
    using (dataFilter.Disable<ISoftDelete>())
    {
        // 临时禁用软删除过滤，查询出已经被标记删除的数据
        var list = (await repository.GetListAsync()).ToList();
        return System.Text.Json.JsonSerializer.Serialize(list);
    }
}
```