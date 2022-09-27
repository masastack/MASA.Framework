中 | [EN](README.md)

## Masa.Contrib.ReadWriteSplitting.Cqrs

用例：

``` powershell
Install-Package Masa.Contrib.Dispatcher.Events //提供进程内事件（本地事件），支持事件编排、Saga、中间件，结合`UoW`使用，确保操作的原子性
Install-Package Masa.Contrib.ReadWriteSplitting.Cqrs
```

### 入门

1. 定义了Command与Query基类
2. 支持Saga模式的Handler，并提供CommandHandler基础实现

#### Query：

1. 定义Query

```C#
public class CatalogItemQuery : Query<List<CatalogItem>>
{
    public string Name { get; set; } = default!;

    public override List<CatalogItem> Result { get; set; } = default!;
}
```

2. 定义QueryHandler

```C#
public class CatalogQueryHandler : QueryHandler<CatalogItemQuery, List<CatalogItem>>
{
    private readonly ICatalogItemRepository _catalogItemRepository;

    public CatalogQueryHandler(ICatalogItemRepository catalogItemRepository)
        => _catalogItemRepository = catalogItemRepository;

    public async Task HandleAsync(CatalogItemQuery query)
    {
        query.Result = await _catalogItemRepository.GetListAsync(query.Name);
    }
}
```

3. 发送Query

```c#
IEventBus eventBus;//通过DI得到IEventBus
await eventBus.PublishAsync(new CatalogItemQuery() { Name = "Rolex" });
```

> 提示：Query后的泛型与Result的返回类型保持一致，需要再Handler中为Result赋值，以便调用方得到结果

#### Command

1. 定义 Command

```c#
public class CreateCatalogItemCommand : Command
{
    public string Name { get; set; } = default!;

    //todo
}
```

2. 添加 CommandHandler

```c#
public class CatalogCommandHandler : CommandHandler<CreateCatalogItemCommand>
{
    private readonly ICatalogItemRepository _catalogItemRepository;

    public CatalogCommandHandler(ICatalogItemRepository catalogItemRepository) => _catalogItemRepository =    catalogItemRepository;

    public async Task HandleAsync(CreateCatalogItemCommand command)
    {
        //todo
    }
}
```

3. 发送 Command

```C#
IEventBus eventBus;//通过DI得到IEventBus
await eventBus.PublishAsync(new CreateCatalogItemCommand());
```
