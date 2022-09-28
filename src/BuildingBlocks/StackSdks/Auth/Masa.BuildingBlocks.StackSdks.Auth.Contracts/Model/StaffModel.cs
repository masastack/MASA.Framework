// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class StaffModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? CurrentTeamId { get; set; }

    public string Department { get; set; } = "";

    public string Position { get; set; } = "";

    public string JobNumber { get; set; } = "";

    public StaffTypes StaffType { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string IdCard { get; set; } = "";

    public string Account { get; set; } = "";

    public string CompanyName { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public AddressValueModel Address { get; set; } = new();

    public GenderTypes Gender { get; set; }

    public bool Enabled { get; set; }

    public StaffModel()
    {
    }

    public StaffModel(
        Guid id,
        Guid userId,
        string department,
        string position,
        string jobNumber,
        StaffTypes staffType,
        string name,
        string displayName,
        string avatar,
        string idCard,
        string account,
        string companyName,
        string phoneNumber,
        string email,
        AddressValueModel address,
        GenderTypes gender,
        bool enabled)
    {
        Id = id;
        UserId = userId;
        Department = department;
        Position = position;
        JobNumber = jobNumber;
        StaffType = staffType;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        Account = account;
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        Gender = gender;
        Enabled = enabled;
    }
}

