// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Service;

public interface ILoginService
{
    Task<TokenModel> LoginByPasswordAsync(LoginByPasswordModel login);

    Task<TokenModel> LoginByPhoneNumberAsync(LoginByPhoneNumberFromSsoModel login);

    Task<LoginByThirdPartyIdpResultModel> LoginByThirdPartyIdpAsync(LoginByThirdPartyIdpModel login);
}
