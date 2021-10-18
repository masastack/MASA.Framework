## Contracts.EF

Example：

```C#
Install-Package MASA.Contrib.Data.Contracts.EF
```

```C#
builder.Services
    .AddUoW<CustomDbContext>(dbOptions =>
    {
        dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity");
        dbOptions.UseSoftDelete(builder.Services);
    })
```

> When the entity inherits ISoftware and is deleted, change the delete state to the modified state, and cooperate with the custom Remove operation to achieve soft deletion
> Do not query the data marked as soft deleted when querying
> When combined with EventBus, the transaction is opened after the first CUD, and the transaction rollback is supported when the entire Handler is abnormal.
