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

    public async Task<UserModel?> UpsertAsync(UpsertUserModel user)
    {
        var requestUri = $"api/user/upsertExternal";
        return await _callerProvider.PostAsync<UpsertUserModel, UserModel>(requestUri, user);
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

    public async Task<long> GetTotalByDepartmentAsync(Guid departmentId)
    {
        var requestUri = $"api/staff/getTotalByDepartment";
        return await _callerProvider.GetAsync<object, long>(requestUri, new { id = departmentId });
    }

    public async Task<long> GetTotalByRoleAsync(Guid roleId)
    {
        var requestUri = $"api/staff/getTotalByRole";
        return await _callerProvider.GetAsync<object, long>(requestUri, new { id = roleId });
    }

    public async Task<long> GetTotalByTeamAsync(Guid teamId)
    {
        var requestUri = $"api/staff/getTotalByTeam";
        return await _callerProvider.GetAsync<object, long>(requestUri, new { id = teamId });
    }

    public async Task<bool> ValidateCredentialsByAccountAsync(string account, string password)
    {
        var requestUri = $"api/user/validateByAccount";
        return await _callerProvider.PostAsync<object, bool>(requestUri, new { account, password });
    }

    public async Task<UserModel> FindByAccountAsync(string account)
    {
        var requestUri = $"api/user/findByAccount";
        return await _callerProvider.GetAsync<object, UserModel>(requestUri, new { account }) ?? new();
    }

    public async Task<UserModel?> FindByPhoneNumberAsync(string phoneNumber)
    {
        var requestUri = $"api/user/findByPhoneNumber";
        return await _callerProvider.GetAsync<object, UserModel>(requestUri, new { phoneNumber });
    }

    public async Task<UserModel?> FindByEmailAsync(string email)
    {
        var requestUri = $"api/user/findByEmail";
        return await _callerProvider.GetAsync<object, UserModel>(requestUri, new { email });
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

    public async Task<List<UserVisitedModel>> GetVisitedListAsync()
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/visitedList";
        return (await _callerProvider.GetAsync<object, List<UserVisitedModel>>(requestUri, new { userId = userId })) ?? new();
    }

    public async Task UpdatePasswordAsync(UpdateUserPasswordModel user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/updatePassword";
        await _callerProvider.PutAsync(requestUri, user);
    }

    public async Task UpdateBasicInfoAsync(UpdateUserBasicInfoModel user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/updateBasicInfo";
        await _callerProvider.PutAsync(requestUri, user);
    }

    public async Task<List<UserPortraitModel>> GetUserPortraitsAsync(params Guid[] userIds)
    {
        var requestUri = $"api/user/portraits";
        return await _callerProvider.PostAsync<Guid[], List<UserPortraitModel>>(requestUri, userIds) ?? new();
    }
}

