// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class SendEmailModel : IEnvironmentModel
{
    public string Email { get; set; } = "";

    public SendEmailTypes SendEmailType { get; set; } = SendEmailTypes.Undefined;

    public string Environment { get; set; }

    public SendEmailModel()
    {
    }

    public SendEmailModel(string email, SendEmailTypes sendEmailType)
    {
        Email = email;
        SendEmailType = sendEmailType;
    }
}
