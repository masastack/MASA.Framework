// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Tests.Requesties;

[XmlRoot]
public class RegisterUser
{
    [XmlElement]
    public string Account { get; set; } = default!;

    [XmlElement]
    public string Password { get; set; } = default!;

    public RegisterUser() { }

    public RegisterUser(string account, string password) : this()
    {
        Account = account;
        Password = password;
    }
}
