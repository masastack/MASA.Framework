// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class MessageInfoUpsertModel
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Markdown { get; set; } = string.Empty;

    public bool IsJump { get; set; }

    public string JumpUrl { get; set; } = string.Empty;
}
