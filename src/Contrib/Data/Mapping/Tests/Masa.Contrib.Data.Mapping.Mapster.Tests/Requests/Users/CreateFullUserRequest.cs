// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Tests.Requests.Users;

public class CreateFullUserRequest : CreateUserRequest
{
    public int Age { get; set; }

    public string Description { get; set; } = default!;

    public DateTime Birthday { get; set; }

    public AddressItemRequest Hometown { get; set; } = default!;
}
