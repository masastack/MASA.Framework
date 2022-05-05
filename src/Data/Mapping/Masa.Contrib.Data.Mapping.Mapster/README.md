[中](README.zh-CN.md) | EN

## Masa.Contrib.Data.Mapping.Mapster

Masa.Contrib.Data.Mapping.Mapster is an object-to-object mapper based on [Mapster](https://github.com/MapsterMapper/Mapster). It adds automatic acquisition and uses the best constructor mapping on the original basis. Nested mapping is supported to reduce the workload of mapping.

## Example:

1. Install `Masa.Contrib.Data.Mapping.Mapster`

    ````c#
    Install-Package Masa.Contrib.Data.Mapping.Mapster
    ````

2. Using `Mapping`

    ```` C#
    builder.Services.AddMapping();
    ````

3. Mapping objects

    ````
    IMapping mapping;// Get through DI

    var request = new
    {
        Name = "Teach you to learn Dapr...",
        OrderItem = new OrderItem("Teach you to learn Dapr hand by hand", 49.9m)
    };
    var order = mapping.Map<Order>(request);// Map the request to a new object, Parameters with the same attribute name and type of the source object and the target object will be automatically mapped, or the constructor parameter name (case-insensitive) and type of the target object are the same as those of the source object, and they will be mapped through the constructor
    ````

    Mapping class `Order`:

    ```` Order.cs
    public class Order
    {
        public string Name { get; set; }

        public decimal TotalPrice { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public Order(string name)
        {
            Name = name;
        }

        public Order(string name, OrderItem orderItem) : this(name)
        {
            OrderItems = new List<OrderItem> { orderItem };
            TotalPrice = OrderItems.Sum(item => item.Price * item.Number);
        }
    }

    public class OrderItem
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Number { get; set; }

        public OrderItem(string name, decimal price) : this(name, price, 1)
        {

        }

        public OrderItem(string name, decimal price, int number)
        {
            Name = name;
            Price = price;
            Number = number;
        }
    }
    ````