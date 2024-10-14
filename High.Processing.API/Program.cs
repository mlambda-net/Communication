using High.Processing.Domain.Persistency;
using High.Processing.Domain.Services;
using High.Processing.Infrastructure.Api.Telegraphy;
using High.Processing.Infrastructure.DataBase;
using High.Processing.Infrastructure.Event;
using MongoDB.Driver;
using Prometheus;

namespace High.Processing.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(5000);
        });
        
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        

        var settings = new MongoSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            Database = "MiniStore",
        };

        builder.Services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(settings.ConnectionString));
        builder.Services.AddSingleton<IEventSender, EventSender>();
        builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>( );
        
        var app = builder.Build();
        
        app.UseRouting();
        app.UseAuthorization();
        app.UseHttpsRedirection();
        app.UseHttpMetrics();
        app.MapControllers();
        app.UseMiddleware<MetricsMiddleware>();

        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
        }
        
     
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMetrics();
        });

        
        
        app.Run();
    }
}