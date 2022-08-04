[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EFCore

## Example:

```c#
Install-Package Masa.Contrib.Data.EFCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer
```

#### Basic usage:

Using MasaDbContext

``` C#
builder.Services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
{
    optionsBuilder.UseFilter();//enable filtering
    optionsBuilder.DbContextOptionsBuilder.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
});
```

Recommended usage:

- [SqlServer](../Masa.Contrib.Data.EFCore.SqlServer/README.md)
- [MySql](../Masa.Contrib.Data.EFCore.MySql/README.md)
- [Pomelo.MySql](../Masa.Contrib.Data.EFCore.Pomelo.MySql/README.md)
- [Sqlite](../Masa.Contrib.Data.EFCore.Sqlite/README.md)
- [Cosmos](../Masa.Contrib.Data.EFCore.Cosmos/README.md)
- [InMemory](../Masa.Contrib.Data.EFCore.InMemory/README.md)
- [Oracle](../Masa.Contrib.Data.EFCore.Oracle/README.md)
- [PostgreSql](../Masa.Contrib.Data.EFCore.PostgreSql/README.md)

#### data filter

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