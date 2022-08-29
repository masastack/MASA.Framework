// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Service;

public class UserService : IUserService
{
    readonly ICaller _caller;
    readonly IUserContext _userContext;

    public UserService(ICaller caller, IUserContext userContext)
    {
        _caller = caller;
        _userContext = userContext;
    }

    public async Task<UserModel?> AddAsync(AddUserModel user)
    {
        var requestUri = $"api/user/addExternal";
        return await _caller.PostAsync<AddUserModel, UserModel>(requestUri, user);
    }

    public async Task<UserModel?> UpsertThirdPartyUserAsync(UpsertThirdPartyUserModel user)
    {
        var requestUri = $"api/thirdPartyUser/upsertThirdPartyUserExternal";
        return await _caller.PostAsync<UpsertThirdPartyUserModel, UserModel>(requestUri, user);
    }

    public async Task<UserModel?> UpsertAsync(UpsertUserModel user)
    {
        var requestUri = $"api/user/upsertExternal";
        return await _caller.PostAsync<UpsertUserModel, UserModel>(requestUri, user);
    }

    public async Task<List<StaffModel>> GetListByDepartmentAsync(Guid departmentId)
    {
        var requestUri = $"api/staff/getListByDepartment";
        return await _caller.GetAsync<object, List<StaffModel>>(requestUri, new { id = departmentId }) ?? new();
    }

    public async Task<List<StaffModel>> GetListByRoleAsync(Guid roleId)
    {
        var requestUri = $"api/staff/getListByRole";
        return await _caller.GetAsync<object, List<StaffModel>>(requestUri, new { id = roleId }) ?? new();
    }

    public async Task<List<StaffModel>> GetListByTeamAsync(Guid teamId)
    {
        var requestUri = $"api/staff/getListByTeam";
        return await _caller.GetAsync<object, List<StaffModel>>(requestUri, new { id = teamId }) ?? new();
    }

    public async Task<long> GetTotalByDepartmentAsync(Guid departmentId)
    {
        var requestUri = $"api/staff/getTotalByDepartment";
        return await _caller.GetAsync<object, long>(requestUri, new { id = departmentId });
    }

    public async Task<long> GetTotalByRoleAsync(Guid roleId)
    {
        var requestUri = $"api/staff/getTotalByRole";
        return await _caller.GetAsync<object, long>(requestUri, new { id = roleId });
    }

    public async Task<long> GetTotalByTeamAsync(Guid teamId)
    {
        var requestUri = $"api/staff/getTotalByTeam";
        return await _caller.GetAsync<object, long>(requestUri, new { id = teamId });
    }

    public async Task<bool> ValidateCredentialsByAccountAsync(string account, string password, bool isLdap = false)
    {
        var requestUri = $"api/user/validateByAccount";
        return await _caller.PostAsync<object, bool>(requestUri, new { account, password, isLdap });
    }

    public async Task<UserModel?> FindByAccountAsync(string account)
    {
        var requestUri = $"api/user/findByAccount";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { account });
    }

    public async Task<UserModel?> FindByPhoneNumberAsync(string phoneNumber)
    {
        var requestUri = $"api/user/findByPhoneNumber";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { phoneNumber });
    }

    public async Task<UserModel?> FindByEmailAsync(string email)
    {
        var requestUri = $"api/user/findByEmail";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { email });
    }

    public async Task<UserModel> GetCurrentUserAsync()
    {
        var id = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/findById";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { id }) ?? new();
    }

    public async Task<StaffDetailModel?> GetCurrentStaffAsync()
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/staff/getDetailByUserId";
        return await _caller.GetAsync<object, StaffDetailModel>(requestUri, new { userId });
    }

    public async Task VisitedAsync(string appId, string url)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/visit";
        await _caller.PostAsync<object>(requestUri, new { UserId = userId, appId = appId, Url = url }, true);
    }

    public async Task<List<UserVisitedModel>> GetVisitedListAsync()
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/visitedList";
        return (await _caller.GetAsync<object, List<UserVisitedModel>>(requestUri, new { userId = userId })) ?? new();
    }

    public async Task UpdatePasswordAsync(UpdateUserPasswordModel user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/updatePassword";
        await _caller.PutAsync(requestUri, user);
    }

    public async Task UpdateUserAvatarAsync(UpdateUserAvatarModel user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/updateAvatar";
        await _caller.PutAsync(requestUri, user);
    }

    public async Task UpdateStaffAvatarAsync(UpdateStaffAvatarModel staff)
    {
        if (staff.UserId == Guid.Empty)
        {
            staff.UserId = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/staff/updateAvatar";
        await _caller.PutAsync(requestUri, staff);
    }

    public async Task UpdateBasicInfoAsync(UpdateUserBasicInfoModel user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/updateBasicInfo";
        await _caller.PutAsync(requestUri, user);
    }

    public async Task UpdateStaffBasicInfoAsync(UpdateStaffBasicInfoModel staff)
    {
        if (staff.UserId == Guid.Empty)
        {
            staff.UserId = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/staff/updateBasicInfo";
        await _caller.PutAsync(requestUri, staff);
    }

    public async Task<List<UserPortraitModel>> GetUserPortraitsAsync(params Guid[] userIds)
    {
        var requestUri = $"api/user/portraits";
        return await _caller.PostAsync<Guid[], List<UserPortraitModel>>(requestUri, userIds) ?? new();
    }

    public async Task SaveUserSystemDataAsync<T>(string systemId, T data)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/UserSystemData";
        await _caller.PostAsync<object>(requestUri,
            new { UserId = userId, SystemId = systemId, Data = JsonSerializer.Serialize(data) },
            true);
    }

    public async Task<T?> GetUserSystemDataAsync<T>(string systemId)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/GetUserSystemData";
        var data = await _caller.GetAsync<object, string>(requestUri, new { userId = userId, systemId = systemId });
        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task<bool> DisableUserAsync(DisableUserModel user)
    {
        var requestUri = $"api/user/disable";
        return await _caller.PutAsync<bool>(requestUri, user);
    }

    public async Task<List<UserSimpleModel>> GetListByAccountAsync(IEnumerable<string> accounts)
    {
        var requestUri = $"api/user/getListByAccount";
        return await _caller.GetAsync<object, List<UserSimpleModel>>(requestUri, new { accounts = string.Join(',', accounts) }) ?? new();
    }

    public async Task SendMsgCodeAsync(SendMsgCodeModel model)
    {
        if (model.UserId == Guid.Empty)
        {
            model.UserId = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/sendMsgCode";
        await _caller.PostAsync(requestUri, model);
    }

    public async Task<bool> VerifyMsgCodeAsync(VerifyMsgCodeModel model)
    {
        if (model.UserId == Guid.Empty)
        {
            model.UserId = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/verifyMsgCode";
        return await _caller.PostAsync<bool>(requestUri, model);
    }

    public async Task<bool> UpdatePhoneNumberAsync(UpdateUserPhoneNumberModel user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/updatePhoneNumber";
        return await _caller.PutAsync<bool>(requestUri, user);
    }
}

