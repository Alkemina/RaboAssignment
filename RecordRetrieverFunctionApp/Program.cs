using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecordRetrieverFunctionApp;

internal class Program
{
    /// <summary>
    /// Dependency Injection class
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(s =>
            {

                var sqlConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings:SQLAZURECONNSTR_MyConnectionString");
                s.AddDbContext<RecordContext>(options => options.UseSqlServer(sqlConnectionString));

                var sbConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings:ServiceBusConnectionString");
                s.AddSingleton((s) =>
                {
                    return new ServiceBusClient(sbConnectionString, new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets });
                });

                s.AddScoped<IRecordSender, RecordSender>();
                s.AddScoped<IRecordRepository, RecordRepository>();
            })
            .Build();
        host.Run();
    }
}