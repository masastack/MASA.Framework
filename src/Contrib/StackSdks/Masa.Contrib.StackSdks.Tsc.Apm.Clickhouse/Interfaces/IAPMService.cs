// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse;

public interface IApmService : IDisposable
{
    /// <summary>
    /// 服务列表页，服务详情页endpoints和instance公用
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PaginatedListBase<ServiceListDto>> ServicePageAsync(BaseApmRequestDto query);

    /// <summary>
    /// trace列表
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PaginatedListBase<EndpointListDto>> EndpointPageAsync(BaseApmRequestDto query);

    /// <summary>
    /// 可共用，service和endpoint公用
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<IEnumerable<ChartLineDto>> ChartDataAsync(BaseApmRequestDto query);

    /// <summary>
    /// endpoint 加载耗时分布
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<EndpointLatencyDistributionDto> EndpointLatencyDistributionAsync(ApmEndpointRequestDto query);

    /// <summary>
    /// tendpoint trace tree line
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PaginatedListBase<TraceResponseDto>> TraceLatencyDetailAsync(ApmTraceLatencyRequestDto query);

    /// <summary>
    /// 错误列表
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PaginatedListBase<ErrorMessageDto>> ErrorMessagePageAsync(ApmEndpointRequestDto query);

    /// <summary>
    /// 获取trace下的错误信息统计，按照spanId
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<IEnumerable<ChartPointDto>> GetTraceErrorsAsync(ApmEndpointRequestDto query);

    Task<IEnumerable<ChartLineCountDto>> GetErrorChartAsync(ApmEndpointRequestDto query);

    Task<IEnumerable<ChartLineCountDto>> GetEndpointChartAsync(ApmEndpointRequestDto query);

    Task<IEnumerable<ChartLineCountDto>> GetLogChartAsync(ApmEndpointRequestDto query);
}
