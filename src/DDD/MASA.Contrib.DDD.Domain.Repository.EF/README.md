[中](README.zh-CN.md) | EN

## MASA.Contrib.DDD.Domain.Repository.EF

Example：

```c#
Install-Package MASA.Contrib.DDD.Domain.Repository.EF
```

> Advantages: The EF version of IRepository provides basic CRUD

1. Add Repository.EF

```c#
builder.Services
.AddDomainEventBus(options =>
{
    options.UseRepository<CustomDbContext>();//Use the EF version of Repository to achieve
}
```

2. How to use

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

If the method defined by IRepository is not enough to support the business, you can customize the Repository

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

In use：

```C#
public async Task<List<ProductItem>> ItemsWithNameAsync(string name, [FromService] IProductRepository productRepository)
{
    return await productRepository.ItemsWithNameAsync(name);
}
```

