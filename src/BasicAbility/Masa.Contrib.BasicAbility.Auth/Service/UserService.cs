// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Service;

public class UserService : IUserService
{
    readonly ICallerProvider _callerProvider;
    readonly IUserContext _userContext;

    public UserService(ICallerProvider callerProvider, IUserContext userContext)
    {
        _callerProvider = callerProvider;
        _userContext = userContext;
    }

    public async Task<UserModel?> AddAsync(AddUserModel user)
    {
        var requestUri = $"api/user/addExternal";
        return await _callerProvider.PostAsync<AddUserModel, UserModel>(requestUri, user);
    }

    public async Task<List<StaffModel>> GetListByDepartmentAsync(Guid departmentId)
    {
        var requestUri = $"api/staff/getListByDepartment";
        return await _callerProvider.GetAsync<object, List<StaffModel>>(requestUri, new { id = departmentId }) ?? new();
    }

    public async Task<List<StaffModel>> GetListByRoleAsync(Guid roleId)
    {
        var requestUri = $"api/staff/getListByRole";
        return await _callerProvider.GetAsync<object, List<StaffModel>>(requestUri, new { id = roleId }) ?? new();
    }

    public async Task<List<StaffModel>> GetListByTeamAsync(Guid teamId)
    {
        var requestUri = $"api/staff/getListByTeam";
        return await _callerProvider.GetAsync<object, List<StaffModel>>(requestUri, new { id = teamId }) ?? new();
    }

    public async Task<bool> ValidateCredentialsByAccountAsync(string account, string password)
    {
        var requestUri = $"api/user/validateByAccount";
        return await _callerProvider.PostAsync<object, bool>(requestUri, new { account = account, password = password });
    }

    public async Task<UserModel> FindByAccountAsync(string account)
    {
        var requestUri = $"api/user/findByAccount";
        return await _callerProvider.GetAsync<object, UserModel>(requestUri, new { account = account }) ?? new();
    }

    public async Task<UserModel> GetCurrentUserAsync()
    {
        var id = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/findById";
        return await _callerProvider.GetAsync<object, UserModel>(requestUri, new { id }) ?? new();
    }

    public async Task VisitedAsync(string url)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/visit";
        await _callerProvider.PostAsync<object>(requestUri, new { UserId = userId, Url = url }, true);
    }

    public async Task<List<UserVisitedModel>> GetUserVisitedListAsync()
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/visitedList";
        return (await _callerProvider.GetAsync<object, List<UserVisitedModel>>(requestUri, new { userId = userId })) ?? new();
    }

    public async Task UpdateUserPasswordAsync(UpdateUserPasswordModel user)
    {
        var requestUri = $"api/user/updateUserPassword";
        await _callerProvider.PostAsync(requestUri, user);
    }

    public async Task UpdateUserBaseInfoAsync(UpdateUserBaseInfoModel user)
    {
        var requestUri = $"api/user/updateUserBaseInfo";
        await _callerProvider.PostAsync(requestUri, user);
    }
}

