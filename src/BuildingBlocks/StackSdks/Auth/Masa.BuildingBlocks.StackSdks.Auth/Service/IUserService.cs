// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Service;

public interface IUserService
{
    Task<List<StaffModel>> GetListByTeamAsync(Guid teamId);

    Task<List<UserModel>> GetListByRoleAsync(Guid roleId);

    Task<List<StaffModel>> GetListByDepartmentAsync(Guid departmentId);

    Task<long> GetTotalByDepartmentAsync(Guid departmentId);

    Task<long> GetTotalByRoleAsync(Guid roleId);

    Task<long> GetTotalByTeamAsync(Guid teamId);

    Task<UserModel> AddAsync(AddUserModel user);

    Task<UserModel> UpsertThirdPartyUserAsync(UpsertThirdPartyUserModel user);

    Task RemoveThirdPartyUserAsync(Guid id);

    Task RemoveThirdPartyUserByThridPartyIdentityAsync(string thridPartyIdentity);

    Task<UserModel> AddThirdPartyUserAsync(AddThirdPartyUserModel user, bool whenExistReturn = true);

    Task<UserModel?> GetThirdPartyUserAsync(GetThirdPartyUserModel model);

    Task<UserModel> UpsertAsync(UpsertUserModel user);

    Task<UserModel?> ValidateAccountAsync(ValidateAccountModel validateAccountModel);

    Task<UserModel?> GetByAccountAsync(string account);

    Task<UserModel?> GetByPhoneNumberAsync(string phoneNumber);

    Task<UserModel?> GetByEmailAsync(string email);

    Task<UserModel?> GetByIdAsync(Guid userId);

    Task<UserModel?> GetCurrentUserAsync();

    Task<StaffDetailModel?> GetCurrentStaffAsync();

    Task VisitedAsync(string appId, string url);

    Task<List<UserVisitedModel>> GetVisitedListAsync();

    Task UpdatePasswordAsync(UpdateUserPasswordModel user);

    Task UpdateUserAvatarAsync(UpdateUserAvatarModel user);

    Task UpdateStaffAvatarAsync(UpdateStaffAvatarModel staff);

    Task SendMsgCodeAsync(SendMsgCodeModel model);

    Task<bool> VerifyMsgCodeAsync(VerifyMsgCodeModel model);

    Task<bool> UpdatePhoneNumberAsync(UpdateUserPhoneNumberModel user);

    Task UpdateBasicInfoAsync(UpdateUserBasicInfoModel user);

    Task UpdateStaffBasicInfoAsync(UpdateStaffBasicInfoModel staff);

    Task<List<UserModel>> GetListByIdsAsync(params Guid[] userIds);

    Task UpsertSystemDataAsync<T>(Guid userId, string systemId, T data);

    Task UpsertSystemDataAsync<T>(string systemId, T data);

    Task<T?> GetSystemDataAsync<T>(string systemId);

    Task<T?> GetSystemDataAsync<T>(Guid userId, string systemId);

    Task<Dictionary<Guid, T?>> GetSystemListDataAsync<T>(IEnumerable<Guid> userIds, string systemId);

    Task<bool> DisableAsync(DisableUserModel user);

    Task<List<UserSimpleModel>> GetListByAccountAsync(IEnumerable<string> accounts);

    Task<UserModel?> LoginByPhoneNumberAsync(LoginByPhoneNumberModel login);

    Task RemoveUserRolesAsync(RemoveUserRolesModel user);

    Task SetCurrentTeamAsync(Guid teamId);

    Task SendEmailAsync(SendEmailModel model);

    Task<UserModel?> RegisterByPhoneAsync(RegisterByPhoneModel model);

    Task<UserModel?> RegisterByEmailAsync(RegisterByEmailModel model);

    Task<bool> HasPasswordAsync(Guid userId = default);

    Task<UserModel> RegisterThirdPartyUserAsync(RegisterThirdPartyUserModel model);

    Task<bool> HasPhoneNumberInEnvAsync(string env, string phoneNumber);

    Task<bool> ResetPasswordByEmailAsync(ResetPasswordByEmailModel resetPasswordByEmailModel);

    Task<bool> ResetPasswordByPhoneAsync(ResetPasswordByPhoneModel resetPasswordByPhoneModel);

    Task RemoveAsync(Guid id);

    Task<List<UserSelectModel>> SearchAsync(string search);

    Task BindRolesAsync(BindUserRolesModel model);

    Task UnbindRolesAsync(UnbindUserRolesModel model);

    Task<Dictionary<string, string>> GetClaimValuesAsync(Guid userId);

    Task AddClaimValuesAsync(UserClaimValuesModel userClaimValuesModel);
}

