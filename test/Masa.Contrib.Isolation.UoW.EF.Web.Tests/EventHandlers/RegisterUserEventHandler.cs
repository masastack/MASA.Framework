using Masa.BuildingBlocks.Isolation.Environment;

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
        var user = new Users()
        {
            Account = @event.Account,
            Password = MD5Utils.Encrypt(@event.Password, @event.Password)
        };
        await _customDbContext.Set<Users>().AddAsync(user);
        await _customDbContext.SaveChangesAsync();

        var user2 = await _customDbContext.Set<Users>().FirstOrDefaultAsync();
        Assert.IsTrue(user2!.Account == @event.Account);
        Assert.IsTrue(user2.Environment == "pro");
        Assert.IsTrue(user2.TenantId == 2);

        _environmentSetter.SetEnvironment("dev"); //In EventHandler, physical isolation is not retriggered if a new DbContext is not recreated, it can only be used to filter changes
        Assert.IsTrue(_environmentContext.CurrentEnvironment == "dev");

        var user3 = await _customDbContext.Set<Users>().FirstOrDefaultAsync();
        Assert.IsNull(user3);

        using (_dataFilter.Disable<IMultiEnvironment>())
        {
            var user4 = await _customDbContext.Set<Users>().FirstOrDefaultAsync();
            Assert.IsNotNull(user4);
        }
    }
}
