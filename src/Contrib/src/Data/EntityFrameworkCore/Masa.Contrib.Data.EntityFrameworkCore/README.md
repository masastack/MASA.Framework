[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.EntityFrameworkCore

## Example:

```c#
Install-Package Masa.Contrib.Data.EntityFrameworkCore
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

- [SqlServer](../Masa.Contrib.Data.EntityFrameworkCore.SqlServer/README.md)
- [MySql](../Masa.Contrib.Data.EntityFrameworkCore.MySql/README.md)
- [Pomelo.MySql](../Masa.Contrib.Data.EntityFrameworkCore.Pomelo.MySql/README.md)
- [Sqlite](../Masa.Contrib.Data.EntityFrameworkCore.Sqlite/README.md)
- [Cosmos](../Masa.Contrib.Data.EntityFrameworkCore.Cosmos/README.md)
- [InMemory](../Masa.Contrib.Data.EntityFrameworkCore.InMemory/README.md)
- [Oracle](../Masa.Contrib.Data.EntityFrameworkCore.Oracle/README.md)
- [PostgreSql](../Masa.Contrib.Data.EntityFrameworkCore.PostgreSql/README.md)

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