using System.Diagnostics;
using High.Processing.Domain.Events;
using High.Processing.Infrastructure.DataBase;
using Prometheus;
using High.Processing.Infrastructure.Event;
using MongoDB.Driver;

namespace High.Processing.App;

public class Worker
{
    private readonly MetricServer _metricServer;
    private readonly EventOrchestrator _orchestrator;
    private readonly Counter _requestCount;
    private readonly Histogram _requestDuration;
    private readonly Gauge _liveRequest;


    public Worker()
    {
        _metricServer = new MetricServer(port: 8081);
        _orchestrator = new EventOrchestrator();

        _requestCount = Metrics.CreateCounter("product_call_total", "HTTP Requests Total", new CounterConfiguration
        {
            LabelNames = ["method", "handler"]
        });
        _requestDuration = Metrics.CreateHistogram("product_call_duration", "HTTP Requests Duration",
            new HistogramConfiguration
            {
                LabelNames = ["method", "handler"]
            });
        _liveRequest = Metrics.CreateGauge("product_call_live", "HTTP Requests Live", new GaugeConfiguration
        {
            LabelNames = ["method", "handler"]
        });
    }

    public async Task Configure()
    {
        var settings = new MongoSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            Database = "MiniStore",
        };
        
        var client = new MongoClient(settings.ConnectionString);
        var uow = new UnitOfWork(client);
        var handler = new PersistentHandler(uow);
        await _orchestrator.Handler(Intercept<CreateProduct>("CreateProduct", "store", handler.Create));
        await _orchestrator.Handler(Intercept<UpdateProduct>("UpdateProduct", "store", handler.Update));
        await _orchestrator.Handler(Intercept<DeleteProduct>("DeleteProduct", "store", handler.Delete));

        var logger = new LogHandler();
        await _orchestrator.Handler(Intercept<CreateProduct>("CreateProduct", "logger", logger.Create));
        await _orchestrator.Handler(Intercept<UpdateProduct>("UpdateProduct", "logger", logger.Update));
        await _orchestrator.Handler(Intercept<DeleteProduct>("DeleteProduct", "logger", logger.Delete));

        var cipher = new CipherHandler();
        await _orchestrator.Handler(Intercept<CreateProduct>("CreateProduct", "cipher", logger.Create));


    }

    private Func<T, Task> Intercept<T>(string method, string handlerName, Func<T, Task> handler)
    {
        return async msg =>
        {
            _requestCount.WithLabels(method, handlerName).Inc();
            _liveRequest.WithLabels(method, handlerName).Inc();
            var stopwatch = Stopwatch.StartNew();
            await handler(msg);
            _liveRequest.WithLabels(method, handlerName).Dec();
            stopwatch.Stop();
            _requestDuration.WithLabels(method, handlerName).Observe(stopwatch.Elapsed.TotalMilliseconds);
        };
    }


    public void Start()
    {
        _metricServer.Start();
        _orchestrator.Start();
    }

    public void Stop()
    {
        _metricServer.Stop();
        _orchestrator.Stop();
        _orchestrator.Dispose();
    }
    
}