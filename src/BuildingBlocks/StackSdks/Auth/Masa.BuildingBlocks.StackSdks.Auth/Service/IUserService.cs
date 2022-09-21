// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Service;

public interface IUserService
{
    Task<List<StaffModel>> GetListByTeamAsync(Guid teamId);

    Task<List<StaffModel>> GetListByRoleAsync(Guid roleId);

    Task<List<StaffModel>> GetListByDepartmentAsync(Guid departmentId);

    Task<long> GetTotalByDepartmentAsync(Guid departmentId);

    Task<long> GetTotalByRoleAsync(Guid roleId);

    Task<long> GetTotalByTeamAsync(Guid teamId);

    Task<UserModel> AddAsync(AddUserModel user);

    Task<UserModel> UpsertThirdPartyUserAsync(UpsertThirdPartyUserModel user);

    Task<UserModel> AddThirdPartyUserAsync(AddThirdPartyUserModel user, bool whenExistReturn = true);

    Task<UserModel> UpsertAsync(UpsertUserModel user);

    Task<bool> ValidateCredentialsByAccountAsync(string account, string password, bool isLdap = false);

    Task<UserModel?> FindByAccountAsync(string account);

    Task<UserModel?> FindByPhoneNumberAsync(string phoneNumber);

    Task<UserModel?> FindByEmailAsync(string email);

    Task<UserModel> GetCurrentUserAsync();

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

    Task UpdateStaffBasicInfoAsync(UpdateStaffBasicInfoModel user);

    Task<List<UserPortraitModel>> GetUserPortraitsAsync(params Guid[] userIds);

    Task SaveUserSystemDataAsync<T>(string systemId, T data);

    Task<T?> GetUserSystemDataAsync<T>(string systemId);

    Task<bool> DisableUserAsync(DisableUserModel user);

    Task<List<UserSimpleModel>> GetListByAccountAsync(IEnumerable<string> accounts);

    Task<UserModel> LoginByPhoneNumberAsync(LoginByPhoneNumberModel login);

    Task<string> LoginByPhoneNumberFromSsoAsync(string address, LoginByPhoneNumberFromSso login);

    Task RemoveUserRolesAsync(RemoveUserRolesModel user);

    Task SetCurrentTeamAsync(Guid teamId);

    Task SendEmailAsync(SendEmailModel model);

    Task RegisterByPhoneAsync(RegisterByPhoneModel model);

    Task RegisterByEmailAsync(RegisterByEmailModel model);
}

