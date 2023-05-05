// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Mc.Model;

public class GetMessageTaskModel : PaginatedOptions
{
    public string Filter { get; set; } = string.Empty;

    public Guid ChannelId { get; set; }

    public string ChannelCode { get; set; } = string.Empty;

    public MessageTypes? EntityType { get; set; }

    public bool? IsDraft { get; set; }

    public bool? IsEnabled { get; set; }

    public MessageTaskTimeTypes? TimeType { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public MessageTaskStatuses? Status { get; set; }

    public MessageTaskSources? Source { get; set; }

    public string SystemId { get; set; } = string.Empty;

    public GetMessageTaskModel(int page, int pageSize, Dictionary<string, bool>? sorting = null) : base(page, pageSize, sorting)
    {

    }
}
