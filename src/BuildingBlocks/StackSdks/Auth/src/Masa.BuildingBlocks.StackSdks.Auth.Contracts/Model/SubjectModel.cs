// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class SubjectModel
{
    public Guid SubjectId { get; set; }

    public string Name { get; set; }

    public string? DisplayName { get; set; }

    public string? Avatar { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public SubjectTypes SubjectType { get; set; }

    public SubjectModel()
    {
        Name = "";
    }

    public SubjectModel(
        Guid subjectId,
        string name,
        string? displayName,
        string? avatar,
        string? phoneNumber,
        string? email,
        SubjectTypes subjectType)
    {
        SubjectId = subjectId;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
        SubjectType = subjectType;
    }
}

