[中](README.zh-CN.md) | EN

## Masa.Utils.Data.EntityFrameworkCore

## Example:

```c#
Install-Package Masa.Utils.Data.EntityFrameworkCore
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

- [SqlServer](../Masa.Utils.Data.EntityFrameworkCore.SqlServer/README.md)
- [MySql](../Masa.Utils.Data.EntityFrameworkCore.MySql/README.md)
- [Pomelo.MySql](../Masa.Utils.Data.EntityFrameworkCore.Pomelo.MySql/README.md)
- [Sqlite](../Masa.Utils.Data.EntityFrameworkCore.Sqlite/README.md)
- [Cosmos](../Masa.Utils.Data.EntityFrameworkCore.Cosmos/README.md)
- [InMemory](../Masa.Utils.Data.EntityFrameworkCore.InMemory/README.md)
- [Oracle](../Masa.Utils.Data.EntityFrameworkCore.Oracle/README.md)
- [PostgreSql](../Masa.Utils.Data.EntityFrameworkCore.PostgreSql/README.md)

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