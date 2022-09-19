// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests;

[TestClass]
public class MappingTest : BaseMappingTest
{
    [TestMethod]
    public void TestCreateUserRequestMapToUserReturnUserNameEqualRequestName()
    {
        var request = new CreateUserRequest()
        {
            Name = "Jim",
        };
        var user = _mapper.Map<CreateUserRequest, User>(request);
        Assert.IsNotNull(user);
        Assert.IsTrue(user.Name == request.Name);
    }

    [TestMethod]
    public void TestObjectMapToUserReturnUserIsNotNull()
    {
        var request = new
        {
            Name = "Jim",
            Age = 18,
            Birthday = DateTime.Now,
            Description = "i am jim"
        };

        var user = _mapper.Map<User>(request);
        Assert.IsNotNull(user);
        Assert.AreEqual(request.Name, user.Name);
        Assert.AreEqual(request.Age, user.Age);
        Assert.AreEqual(request.Birthday, user.Birthday);
        Assert.AreEqual(request.Description, user.Description);
    }

    [TestMethod]
    public void TestObjectMapToUserAndSourceParameterGreatherThanDestinationControllerParameterLength()
    {
        var request = new
        {
            Name = "Jim",
            Age = 18,
            Birthday = DateTime.Now,
            Description = "i am jim",
            Tag = Array.Empty<string>()
        };

        var user = _mapper.Map<object, User>(request);
        Assert.IsNotNull(user);
        Assert.AreEqual(request.Name, user.Name);
        Assert.AreEqual(request.Age, user.Age);
        Assert.AreEqual(request.Birthday, user.Birthday);
        Assert.AreEqual(request.Description, user.Description);
    }

    [TestMethod]
    public void TestCreateFullUserRequestMapToUserReturnHometownIsNotNull()
    {
        var request = new CreateFullUserRequest()
        {
            Name = "Jim",
            Age = 18,
            Birthday = DateTime.Now,
            Hometown = new AddressItemRequest()
            {
                Province = "BeiJing",
                City = "BeiJing",
                Address = "National Sport Stadium"
            }
        };

        var user = _mapper.Map<CreateFullUserRequest, User>(request);
        Assert.IsNotNull(user);
        Assert.AreEqual(request.Name, user.Name);
        Assert.AreEqual(request.Age, user.Age);
        Assert.AreEqual(request.Birthday, user.Birthday);
        Assert.AreEqual(request.Description, user.Description);
        Assert.IsNotNull(request.Hometown);
        Assert.AreEqual(request.Hometown.Province, user.Hometown.Province);
        Assert.AreEqual(request.Hometown.City, user.Hometown.City);
        Assert.AreEqual(request.Hometown.Address, user.Hometown.Address);
    }

    [TestMethod]
    public void TestOrderRequestMapToOrderReturnTotalPriceIs10()
    {
        var request = new OrderRequest()
        {
            Name = "orderName",
            OrderItem = new OrderItem("apple", 10)
        };

        var order = _mapper.Map<OrderRequest, Order>(request);
        Assert.IsNotNull(order);
        Assert.AreEqual(request.Name, order.Name);
        Assert.AreEqual(1, order.OrderItems.Count);
        Assert.AreEqual(request.OrderItem.Name, order.OrderItems[0].Name);
        Assert.AreEqual(request.OrderItem.Price, order.OrderItems[0].Price);
        Assert.AreEqual(1, order.OrderItems[0].Number);
        Assert.AreEqual(1 * 10, order.TotalPrice);
    }

    [TestMethod]
    public void TestOrderMultiRequestMapToOrderReturnOrderItemsCountIs1AndTotalPriceIs10()
    {
        var request = new
        {
            Name = "Order Name",
            OrderItems = new List<OrderItem>()
            {
                new("Apple", 10)
            }
        };

        var order = _mapper.Map<Order>(request);
        Assert.IsNotNull(order);
        Assert.AreEqual(request.Name, order.Name);
        Assert.AreEqual(1, order.OrderItems.Count);
        Assert.AreEqual(request.OrderItems[0].Name, order.OrderItems[0].Name);
        Assert.AreEqual(request.OrderItems[0].Price, order.OrderItems[0].Price);
        Assert.AreEqual(1, order.OrderItems[0].Number);
        Assert.AreEqual(10, order.TotalPrice);
    }

    [TestMethod]
    public void TestOrderMultiRequestMapToOrderReturnOrderItemsCountIs1()
    {
        var request = new OrderMultiRequest()
        {
            Name = "Order Name",
            OrderItems = new List<OrderItemRequest>()
            {
                new()
                {
                    Name = "Apple",
                    Price = 10,
                    Number = 1
                }
            }
        };

        var order = _mapper.Map<OrderMultiRequest, Order>(request);
        Assert.IsNotNull(order);
        Assert.AreEqual(request.Name, order.Name);
        Assert.AreEqual(1, order.OrderItems.Count);
        Assert.AreEqual(request.OrderItems[0].Name, order.OrderItems[0].Name);
        Assert.AreEqual(request.OrderItems[0].Price, order.OrderItems[0].Price);
        Assert.AreEqual(1, order.OrderItems[0].Number);
        Assert.AreEqual(0, order.TotalPrice);
    }

    [TestMethod]
    public void TestMapToExistingObject()
    {
        var request = new
        {
            Name = "Jim",
            Age = 18
        };
        User user = new User("Time")
        {
            Description = "Description",
        };

        var newUser = _mapper.Map(request, user);
        Assert.IsNotNull(newUser);
        Assert.IsTrue(newUser.Description == "Description");
        Assert.IsTrue(newUser.Name == "Jim");
        Assert.IsTrue(newUser.Age == 18);
    }

    [TestMethod]
    public void TestCreateUserRequestListMapToUsers()
    {
        List<CreateUserRequest> requests = new List<CreateUserRequest>()
        {
            new()
            {
                Name = "Jim"
            }
        };
        List<User> users = new();
        var newUsers = _mapper.Map(requests, users);
        Assert.IsTrue(newUsers.Count == 1);
        Assert.IsTrue(newUsers[0].Name == "Jim");
    }

    [TestMethod]
    public void TestNested()
    {
        var user = new UserDto()
        {
            Id = 1,
            Name = "test"
        };
        var user2 = new UserDto()
        {
            Id = 2,
            Name = "test2",
            User = user
        };

        MasaApp.Services = _services;
        var newUser = user2.Map<UserDto>();

        Assert.AreEqual(user2.Id, newUser.Id);
        Assert.AreEqual(user2.Name, newUser.Name);
        Assert.IsNotNull(newUser.User);
        Assert.AreEqual(user.Id, newUser.User.Id);
        Assert.AreEqual(user.Name, newUser.User.Name);
    }

    [TestMethod]
    public void TestNested2()
    {
        var order = new OrderDto()
        {
            Id = 1,
            Name = "test",
            Address = new AddressItemDto()
            {
                Province = "Province1",
                City = "City1",
                Address = new AddressItemDto()
                {
                    Province = "Province2",
                    City = "City2",
                }
            }
        };

        MasaApp.Services = _services;

        var newOrder = order.Map<OrderDto>();

        Assert.AreEqual(order.Id, newOrder.Id);
        Assert.AreEqual(order.Name, newOrder.Name);
        Assert.IsNotNull(newOrder.Address);
        Assert.AreEqual(order.Address.Province, newOrder.Address.Province);
        Assert.AreEqual(order.Address.City, newOrder.Address.City);
    }
}
