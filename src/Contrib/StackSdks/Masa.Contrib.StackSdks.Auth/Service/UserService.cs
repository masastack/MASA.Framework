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

    public async Task<UserModel?> ValidateCredentialsByAccountAsync(string account, string password, bool isLdap = false)
    {
        var requestUri = $"api/user/validateByAccount";
        return await _caller.PostAsync<object, UserModel>(requestUri, new { account, password, isLdap });
    }

    public async Task<UserModel?> FindByAccountAsync(string account)
    {
        var requestUri = $"api/user/byAccount";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { account });
    }

    public async Task<UserModel?> FindByPhoneNumberAsync(string phoneNumber)
    {
        var requestUri = $"api/user/byPhoneNumber";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { phoneNumber });
    }

    public async Task<UserModel?> FindByEmailAsync(string email)
    {
        var requestUri = $"api/user/byEmail";
        return await _caller.GetAsync<object, UserModel>(requestUri, new { email });
    }

    public async Task<UserModel?> FindByIdAsync(Guid userId)
    {
        var user = await _multilevelCacheClient.GetAsync<UserModel>(CacheKeyConsts.UserKey(userId));
        return user;
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

    public async Task<List<UserModel>> GetUsersByIdsAsync(params Guid[] userIds)
    {
        var requestUri = $"api/user/usersByids";
        return await _caller.PostAsync<Guid[], List<UserModel>>(requestUri, userIds) ?? new();
    }

    public async Task SaveUserSystemDataAsync<T>(string systemId, T data)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/systemData";
        await _caller.PostAsync<object>(requestUri,
            new { UserId = userId, SystemId = systemId, Data = JsonSerializer.Serialize(data) },
            true);
    }

    public async Task<T?> GetUserSystemDataAsync<T>(string systemId)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"api/user/systemData";
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
        var requestUri = $"api/user/listByAccount";
        return await _caller.GetAsync<object, List<UserSimpleModel>>(requestUri, new { accounts = string.Join(',', accounts) }) ?? new();
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

    public async Task<TokenModel> LoginByPhoneNumberFromSsoAsync(string address, LoginByPhoneNumberFromSso login)
    {
        using var client = new HttpClient();
        var disco = await client.GetDiscoveryDocumentAsync(address);
        if (disco.IsError)
            throw new UserFriendlyException(disco.Error);

        var paramter = new Dictionary<string, string>
        {
            ["client_Id"] = login.ClientId,
            ["client_secret"] = login.ClientSecret,
            ["grant_type"] = "phone_code",
            ["scope"] = string.Join(' ', login.Scope),
            ["PhoneNumber"] = login.PhoneNumber,
            ["code"] = login.Code
        };

        var tokenResponse = await client.RequestTokenRawAsync(disco.TokenEndpoint, new Parameters(paramter));
        if (tokenResponse.IsError)
            throw new UserFriendlyException(tokenResponse.Error);

        return new TokenModel
        {
            AccessToken = tokenResponse.AccessToken,
            IdentityToken = tokenResponse.IdentityToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn,
        };
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

    public async Task RegisterByPhoneAsync(RegisterByPhoneModel model)
    {
        var requestUri = $"api/user/register";
        await _caller.PostAsync(requestUri, model);
    }

    public async Task RegisterByEmailAsync(RegisterByEmailModel model)
    {
        var requestUri = $"api/user/register";
        await _caller.PostAsync(requestUri, model);
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
}

