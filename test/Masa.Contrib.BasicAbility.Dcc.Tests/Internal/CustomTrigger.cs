// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Dcc.Tests.Internal;

public class CustomTrigger
{
    private JsonSerializerOptions _jsonSerializerOptions;

    public CustomTrigger(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    internal ConfigFormats Formats { get; set; }

    internal string Content { get; set; }

    internal Action<string> Action { get; set; }

    internal void Execute()
    {
        Action?.Invoke(new PublishRelease()
        {
            ConfigFormat = Formats,
            Content = Content
        }.Serialize(_jsonSerializerOptions));
    }
}
