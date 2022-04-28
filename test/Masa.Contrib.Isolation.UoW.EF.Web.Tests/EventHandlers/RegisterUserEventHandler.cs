// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EF.Web.Tests.EventHandlers;

public class RegisterUserEventHandler
{
    private readonly CustomDbContext _customDbContext;
    private readonly IDataFilter _dataFilter;
    private readonly IEnvironmentSetter _environmentSetter;
    private readonly IEnvironmentContext _environmentContext;

    public RegisterUserEventHandler(CustomDbContext customDbContext, IDataFilter dataFilter, IEnvironmentSetter environmentSetter,
        IEnvironmentContext environmentContext)
    {
        _customDbContext = customDbContext;
        _dataFilter = dataFilter;
        _environmentSetter = environmentSetter;
        _environmentContext = environmentContext;
    }

    [EventHandler]
    public async Task RegisterUserAsync(RegisterUserEvent @event)
    {
        await _customDbContext.Database.EnsureCreatedAsync();
        Assert.IsTrue(_customDbContext.Database.GetConnectionString() == "data source=test3");
        var user = new User
        {
            Account = @event.Account,
            Password = MD5Utils.Encrypt(@event.Password, @event.Password)
        };
        await _customDbContext.Set<User>().AddAsync(user);
        await _customDbContext.SaveChangesAsync();

        var user2 = await _customDbContext.Set<User>().FirstOrDefaultAsync();
        Assert.IsTrue(user2!.Account == @event.Account);
        Assert.IsTrue(user2.Environment == "pro");
        Assert.IsTrue(user2.TenantId == 2);

        _environmentSetter.SetEnvironment("dev"); //In EventHandler, physical isolation is not retriggered if a new DbContext is not recreated, it can only be used to filter changes
        Assert.IsTrue(_environmentContext.CurrentEnvironment == "dev");
        Assert.IsTrue(_customDbContext.Database.GetConnectionString() == "data source=test3");

        var user3 = await _customDbContext.Set<User>().FirstOrDefaultAsync();
        Assert.IsNull(user3);

        using (_dataFilter.Disable<IMultiEnvironment>())
        {
            Assert.IsTrue(_customDbContext.Database.GetConnectionString() == "data source=test3");
            var user4 = await _customDbContext.Set<User>().FirstOrDefaultAsync();
            Assert.IsNotNull(user4);
        }
    }
}
