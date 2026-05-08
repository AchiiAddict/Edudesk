using MassTransit;
using WorkerService.Workers; 

var builder = Host.CreateApplicationBuilder(args);

// MassTransit ve RabbitMQ Ayarlarý
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CertificateConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

//4 Ayrý Hosted Worker birbirinden baðýmsýz
builder.Services.AddHostedService<SlaScannerWorker>();
builder.Services.AddHostedService<DailyReportWorker>();

var host = builder.Build();
host.Run();