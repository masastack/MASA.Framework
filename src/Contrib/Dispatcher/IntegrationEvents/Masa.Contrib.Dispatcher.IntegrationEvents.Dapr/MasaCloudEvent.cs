// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class MasaCloudEvent<TData> : CloudEvent
{
    /// <summary>
    /// Initialize a new instance of the <see cref="T:Dapr.CloudEvent`1" /> class.
    /// </summary>
    public MasaCloudEvent(TData data) => this.Data = data;

    /// <summary>CloudEvent 'data' content.</summary>
    public TData Data { get; }

    /// <summary>Gets event data.</summary>
    [JsonPropertyName("datacontenttype")]
    public string DataContentType { get; } = "application/masacloudevents+json";
}
