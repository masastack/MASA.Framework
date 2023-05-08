// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

[ExcludeFromCodeCoverage]
internal class MasaCloudEvent<TData> : CloudEvent
{
    /// <summary>
    /// Initialize a new instance of the <see cref="T:Dapr.CloudEvent`1" /> class.
    /// </summary>
    public MasaCloudEvent(TData data) => this.Data = data;

    /// <summary>CloudEvent 'data' content.</summary>
    public TData Data { get; }

    /// <summary>Gets event data.</summary>
    [JsonPropertyName("datacontenttype")]
    public string DataContentType { get; set; } = DaprConstant.DEFAULT_DATA_CONTENT_TYPE;
}
