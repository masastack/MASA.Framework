[中](README.zh-CN.md) | EN

## Contracts.EF

Example：

```C#
Install-Package Masa.Contrib.Data.UoW.EF
Install-Package Masa.Contrib.Data.Contracts.EF
```

```C#
builder.Services.AddEventBus(options => {
    options.UseUoW<CustomDbContext>(dbOptions => dbOptions.UseSoftDelete().UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"));
});
```

> When the entity inherits ISoftware and is deleted, change the delete state to the modified state, and cooperate with the custom Remove operation to achieve soft deletion
> Do not query the data marked as soft deleted when querying

> Frequently Asked Questions:

- Problem 1: After using UseSoftDelete, there is a problem that the submission cannot be saved

       After using Uow, the transaction will be enabled by default after Add、 Modified、 and Deleted
       and the transaction can be saved normally after the transaction is submitted
       If the EventBus is used, the transaction will be automatically submitted