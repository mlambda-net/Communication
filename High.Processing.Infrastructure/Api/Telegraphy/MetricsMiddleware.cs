using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Prometheus;

namespace High.Processing.Infrastructure.Api.Telegraphy;

public class MetricsMiddleware(RequestDelegate next)
{
    private static readonly Counter RequestCount = Metrics.CreateCounter("http_requests_total", "HTTP Requests Total");

    private static readonly Counter FailedRequests =
        Metrics.CreateCounter("http_requests_failed", "HTTP Requests Failed");

    private static readonly Histogram RequestDuration =
        Metrics.CreateHistogram("http_requests_duration", "HTTP Requests Duration");

    private static readonly Gauge LiveRequest = Metrics.CreateGauge("http_requests_live", "HTTP Requests Live");

    public async Task InvokeAsync(HttpContext context)
    {
        RequestCount.Inc();
        LiveRequest.Inc();
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await next(context);
            stopwatch.Stop();
            RequestDuration.Observe(stopwatch.Elapsed.TotalMilliseconds);
        }
        catch
        {
            FailedRequests.Inc();
        }

        LiveRequest.Dec();
    }
}