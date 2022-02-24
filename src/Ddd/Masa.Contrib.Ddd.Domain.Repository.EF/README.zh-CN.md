中 | [EN](README.md)

## Masa.Contrib.Ddd.Domain.Repository.EF

用例：

```c#
Install-Package Masa.Contrib.Ddd.Domain.Repository.EF
```

> 优势：IRepository的EF版实现，提供了基础的CRUD

1. 添加Repository.EF

```c#
builder.Services
.AddDomainEventBus(options =>
{
    options.UseRepository<CustomDbContext>();//使用Repository的EF版实现
}
```

2. 如何使用

```C#
public class ProductItem
{
    public string Name { get; set; }
}

public class DemoService : ServiceBase
{
    public CatalogService(IServiceCollection services) : base(services)
    {

    }

    public async Task CreateProduct(ProductItem product,[FromService]IRepository<Aggregate.Payment> repository)
    {
        await repository.AddAsync(product);
        await repository.UnitOfWork.SaveChangesAsync();
    }
}
```

如果IRepository定义的方法不足以支撑业务，则可以自定义Repository

```C#
public interface IProductRepository : IRepository<ProductItem>
{
    Task<List<ProductItem>> ItemsWithNameAsync(string name);
}

public class ProductRepository : Repository<CustomDbContext, ProductItem>, IProductRepository
{
    public Task<List<ProductItem>> ItemsWithNameAsync(string name)
    {
        //Todo
    }
}
```

在使用上：

```C#
public async Task<List<ProductItem>> ItemsWithNameAsync(string name, [FromService] IProductRepository productRepository)
{
    return await productRepository.ItemsWithNameAsync(name);
}
```

