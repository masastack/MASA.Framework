namespace Masa.Contrib.BasicAbility.Auth.Service;

public class UserService : IUserService
{
    readonly ICallerProvider _callerProvider;

    public UserService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<UserModel?> AddAsync(AddUserModel user)
    {
        var requestUri = $"api/user/addExternal";
        return await _callerProvider.PostAsync<AddUserModel, UserModel>(requestUri, user);
    }

    public async Task<List<StaffModel>> GetListByDepartmentAsync(Guid departmentId)
    {
        var requestUri = $"api/staff/getListByDepartment";
        return await _callerProvider.GetAsync<object, List<StaffModel>>(requestUri, new { departmentId }) ?? new();
    }

    public async Task<List<StaffModel>> GetListByRoleAsync(Guid roleId)
    {
        var requestUri = $"api/staff/getListByRole";
        return await _callerProvider.GetAsync<object, List<StaffModel>>(requestUri, new { roleId }) ?? new();
    }

    public async Task<List<StaffModel>> GetListByTeamAsync(Guid teamId)
    {
        var requestUri = $"api/staff/getListByTeam";
        return await _callerProvider.GetAsync<object, List<StaffModel>>(requestUri, new { teamId }) ?? new();
    }
}

