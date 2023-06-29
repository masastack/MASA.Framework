// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class UserSelectModel
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string DisplayName { get; set; }

    public string Account { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string Avatar { get; set; }

    public UserSelectModel()
    {
        DisplayName = "";
        Account = "";
        Avatar = "";
    }

    public UserSelectModel(Guid id, string name, string displayName, string account, string phoneNumber, string email, string avatar)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Account = account;
        PhoneNumber = phoneNumber;
        Email = email;
        Avatar = avatar;
    }
}
