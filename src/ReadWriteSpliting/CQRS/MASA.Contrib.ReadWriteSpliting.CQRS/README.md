## CQRS

Example：

```C#
1. Define Command and Query base classes
2. Support the Handler of Saga mode, and provide the basic implementation of CommandHandler
```

```C#
Install-Package MASA.Contrib.ReadWriteSpliting.CQRS
```

##### Query：

1. Define Query

```C#
public class CatalogItemQuery : Query<List<CatalogItem>>
{
	public string Name { get; set; } = default!;

	public override List<CatalogItem> Result { get; set; } = default!;
}
```

2. Define QueryHandler

```C#
public class CatalogQueryHandler : QueryHandler<CatalogItemQuery, List<CatalogItem>>
{
    private readonly ICatalogItemRepository _catalogItemRepository;

    public CatalogQueryHandler(ICatalogItemRepository catalogItemRepository) => _catalogItemRepository = catalogItemRepository;

    public async Task HandleAsync(CatalogItemQuery query)
    {
        query.Result = await _catalogItemRepository.GetListAsync(query.Name);
    }
}
```

3. Send Query

```c#
IEventBus eventBus;//Get IEventBus through DI
await eventBus.PublishAsync(new CatalogItemQuery() { Name = "Rolex" });
```

> Tip: The generic type after Query is consistent with the return type of Result. You need to assign a value to Result in Handler so that the caller can get the result.

##### Command

1. Define Command

```c#
public class CreateCatalogItemCommand : Command
{
	public string Name { get; set; } = default!;

    //todo
}
```

2. Add CommandHandler

```c#
public class CatalogCommandHandler : CommandHandler<CreateCatalogItemCommand>
{
    private readonly ICatalogItemRepository _catalogItemRepository;

    public CatalogCommandHandler(ICatalogItemRepository catalogItemRepository) => _catalogItemRepository = catalogItemRepository;

    public async Task HandleAsync(CreateCatalogItemCommand command)
    {
        //todo
    }
}
```

3. Send Command

```C#
IEventBus eventBus;//Get IEventBus through DI
await eventBus.PublishAsync(new CreateCatalogItemCommand());
```