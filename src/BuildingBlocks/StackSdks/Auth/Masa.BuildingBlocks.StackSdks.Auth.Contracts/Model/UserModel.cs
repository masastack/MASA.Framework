// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class UserModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? DisplayName { get; set; }

    public string Account { get; set; }

    public GenderTypes Gender { get; set; }

    public string Avatar { get; set; }

    public string? IdCard { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? CompanyName { get; set; }

    public string? Department { get; set; }

    public string? Position { get; set; }

    public AddressValueModel Address { get; set; } = new();

    public List<Guid> RoleIds { get; set; } = new();

    public DateTime CreationTime { get; set; }

    public UserModel()
    {
        Name = "";
        Avatar = "";
        Account = "";
    }

    public UserModel(
        Guid id,
        string name,
        string? displayName,
        string account,
        GenderTypes gender,
        string avatar,
        string? idCard,
        string? phoneNumber,
        string? email,
        string? companyName,
        string? department,
        string? position,
        AddressValueModel address)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Account = account;
        Gender = gender;
        Avatar = avatar;
        IdCard = idCard;
        PhoneNumber = phoneNumber;
        Email = email;
        CompanyName = companyName;
        Department = department;
        Position = position;
        Address = address;
    }
}

