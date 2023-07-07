// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Service;

public class UserService : IUserService
{
    readonly ICaller _caller;
    readonly IUserContext _userContext;
    readonly IMultilevelCacheClient _multilevelCacheClient;

    public UserService(ICaller caller, IUserContext userContext, IMultilevelCacheClient multilevelCacheClient)
    {
        _caller = caller;
        _userContext = userContext;
        _multilevelCacheClient = multilevelCacheClient;
    }

    public async Task<UserModel> AddAsync(AddUserModel user)
    {
        var requestUri = $"api/user/external";
        return await _caller.PostAsync<AddUserModel, UserModel>(requestUri, user) ?? throw new UserFriendlyException("operation failed");
    }

    public async Task<UserModel> UpsertThirdPartyUserAsync(UpsertThirdPartyUserModel user)
    {
        var requestUri = $"api/thirdPartyUser/upsertThirdPartyUserExternal";
        return await _caller.PostAsync<UpsertThirdPartyUserModel, UserModel>(requestUri, user) ?? throw new UserFriendlyException("operation failed");
    }

    public async Task<UserModel> UpsertAsync(UpsertUserModel user)
    {
        var requestUri = $"api/user/upsertExternal";
        return await _caller.PostAsync<UpsertUserModel, UserModel>(requestUri, user) ?? throw new UserFriendlyException("operation failed");
    }

    public async Task<List<StaffModel>> GetListByDepartmentAsync(Guid departmentId)
    {
        var requestUri = $"api/staff/getListByDepartment";
        return await _caller.GetAsync<object, List<StaffModel>>(requestUri, new { id = departmentId }) ?? new();
    }

    public async Task<List<UserModel>> GetListByRoleAsync(Guid roleId)
    {
        var requestUri = $"api/user/getListByRole";
        return await _caller.GetAsync<object, List<UserModel>>(requestUri, new { id = roleId }) ?? new();
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

    public async Task<UserModel?> ValidateAccountAsync(ValidateAccountModel validateAccountModel)
    {
        var requestUri = $"api/user/validateByAccount";
        return await _caller.PostAsync<object, UserModel>(requestUri, validateAccountModel);
    }

    public async Task<UserModel?> GetByAccountAsync(string account)
    {
        var requestUri = $"api/user/byAccount";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { account });
    }

    public async Task<List<UserSimpleModel>> GetListByAccountAsync(IEnumerable<string> accounts)
    {
        var requestUri = $"api/user/listByAccount";
        return await _caller.GetAsync<object, List<UserSimpleModel>>(requestUri, new { accounts = string.Join(',', accounts) }) ?? new();
    }

    public async Task<UserModel?> GetByPhoneNumberAsync(string phoneNumber)
    {
        var requestUri = $"api/user/byPhoneNumber";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { phoneNumber });
    }

    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        var requestUri = $"api/user/byEmail";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { email });
    }

    public async Task<UserModel?> GetByIdAsync(Guid userId)
    {
        var user = await _multilevelCacheClient.GetAsync<UserModel>(CacheKeyConsts.UserKey(userId));
        return user;
    }

    public async Task<List<UserModel>> GetListByIdsAsync(params Guid[] userIds)
    {
        var requestUri = $"api/user/byIds";
        return await _caller.PostAsync<Guid[], List<UserModel>>(requestUri, userIds) ?? new();
    }

    public async Task<UserModel?> GetCurrentUserAsync()
    {
        var id = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/byId/{id}";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { id });
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
        var requestUri = $"api/user/password";
        await _caller.PutAsync(requestUri, user);
    }

    public async Task UpdateUserAvatarAsync(UpdateUserAvatarModel user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/avatar";
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
        var requestUri = $"api/user/basicInfo";
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

    public async Task UpsertSystemDataAsync<T>(Guid userId, string systemId, T data)
    {
        var requestUri = "api/user/systemData";
        await _caller.PostAsync<object>(requestUri,
            new { UserId = userId, SystemId = systemId, Data = JsonSerializer.Serialize(data) },
            true);
    }

    public async Task UpsertSystemDataAsync<T>(string systemId, T data)
    {
        var userId = _userContext.GetUserId<Guid>();
        await UpsertSystemDataAsync(userId, systemId, data);
    }

    public async Task<T?> GetSystemDataAsync<T>(string systemId)
    {
        var userId = _userContext.GetUserId<Guid>();
        return await GetSystemDataAsync<T>(userId, systemId);
    }

    public async Task<T?> GetSystemDataAsync<T>(Guid userId, string systemId)
    {
        var dataList = await GetSystemListDataAsync<T>(new List<Guid> { userId }, systemId);
        return dataList.FirstOrDefault().Value ?? default;
    }

    public async Task<Dictionary<Guid, T>> GetSystemListDataAsync<T>(IEnumerable<Guid> userIds, string systemId)
    {
        var requestUri = $"api/user/systemData/byIds";
        var data = await _caller.PostAsync<Dictionary<Guid, T>>(requestUri, new GetSystemDataModel { UserIds = userIds.ToList(), SystemId = systemId }) ?? new();
        return data;
    }

    public async Task<bool> DisableAsync(DisableUserModel user)
    {
        var requestUri = $"api/user/disable";
        return await _caller.PutAsync<bool>(requestUri, user);
    }

    public async Task<bool> UpdatePhoneNumberAsync(UpdateUserPhoneNumberModel user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/phoneNumber";
        return await _caller.PutAsync<bool>(requestUri, user);
    }

    public async Task<UserModel?> LoginByPhoneNumberAsync(LoginByPhoneNumberModel login)
    {
        var requestUri = $"api/user/loginByPhoneNumber";
        return await _caller.PostAsync<UserModel>(requestUri, login);
    }

    public async Task RemoveUserRolesAsync(RemoveUserRolesModel user)
    {
        var requestUri = $"api/user/userRoles";
        await _caller.DeleteAsync(requestUri, user);
    }

    public async Task<UserModel> AddThirdPartyUserAsync(AddThirdPartyUserModel user, bool whenExistReturn = true)
    {
        var requestUri = $"api/thirdPartyUser/addThirdPartyUser?whenExistReturn={whenExistReturn}";
        return await _caller.PostAsync<AddThirdPartyUserModel, UserModel>(requestUri, user) ?? throw new UserFriendlyException("operation failed");
    }

    public async Task<UserModel?> GetThirdPartyUserAsync(GetThirdPartyUserModel model)
    {
        var requestUri = $"api/thirdPartyUser";
        return await _caller.GetAsync<GetThirdPartyUserModel, UserModel>(requestUri, model);
    }

    public async Task SetCurrentTeamAsync(Guid teamId)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/staff/UpdateCurrentTeam";
        await _caller.PutAsync(requestUri, new UpdateCurrentTeamModel
        {
            UserId = userId,
            TeamId = teamId
        });
    }

    public async Task SendMsgCodeAsync(SendMsgCodeModel model)
    {
        if (model.UserId == Guid.Empty)
        {
            model.UserId = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/message/sms";
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

    public async Task SendEmailAsync(SendEmailModel model)
    {
        var requestUri = $"api/message/email";
        await _caller.PostAsync(requestUri, model);
    }

    public async Task<UserModel?> RegisterByPhoneAsync(RegisterByPhoneModel model)
    {
        var requestUri = $"api/user/register";
        return await _caller.PostAsync<UserModel?>(requestUri, model);
    }

    public async Task<UserModel?> RegisterByEmailAsync(RegisterByEmailModel model)
    {
        var requestUri = $"api/user/register";
        return await _caller.PostAsync<UserModel?>(requestUri, model);
    }

    public async Task<bool> HasPasswordAsync(Guid userId = default)
    {
        if (userId == Guid.Empty)
        {
            userId = _userContext.GetUserId<Guid>();
        }
        var requestUri = $"api/user/hasPassword";
        return await _caller.GetAsync<bool>(requestUri, new { userId });
    }

    public async Task<UserModel> RegisterThirdPartyUserAsync(RegisterThirdPartyUserModel model)
    {
        var requestUri = $"api/thirdPartyUser/register";
        return await _caller.PostAsync<UserModel>(requestUri, model) ?? throw new UserFriendlyException("Register failed");
    }

    public async Task<bool> HasPhoneNumberInEnvAsync(string env, string phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(env);
        ArgumentNullException.ThrowIfNull(phoneNumber);
        var requestUri = $"api/user/HasPhoneNumberInEnv?env={env}&phoneNumber={phoneNumber}";
        return await _caller.GetAsync<bool>(requestUri);
    }

    public async Task<bool> ResetPasswordByEmailAsync(ResetPasswordByEmailModel resetPasswordByEmailModel)
    {
        var requestUri = $"api/user/reset_password_by_email";
        return await _caller.PostAsync<ResetPasswordByEmailModel, bool>(requestUri, resetPasswordByEmailModel);
    }

    public async Task<bool> ResetPasswordByPhoneAsync(ResetPasswordByPhoneModel resetPasswordByPhoneModel)
    {
        var requestUri = $"api/user/reset_password_by_phone";
        return await _caller.PostAsync<ResetPasswordByPhoneModel, bool>(requestUri, resetPasswordByPhoneModel);
    }

    public async Task RemoveAsync(Guid id)
    {
        var requestUri = "api/user";
        await _caller.DeleteAsync(requestUri, new RemoveUserModel(id));
    }

    public async Task<List<UserSelectModel>> SearchAsync(string search)
    {
        var requestUri = $"api/user/select?search={search}";
        return await _caller.GetAsync<List<UserSelectModel>>(requestUri) ?? new();
    }

    public Task BindRolesAsync(BindUserRolesModel model)
    {
        var requestUri = $"api/user/bind_roles";
        return _caller.PostAsync(requestUri, model);
    }

    public Task UnbindRolesAsync(UnbindUserRolesModel model)
    {
        var requestUri = $"api/user/unbind_roles";
        return _caller.PostAsync(requestUri, model);
    }
}

