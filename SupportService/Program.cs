using MassTransit;
using Microsoft.EntityFrameworkCore;
using SupportService.Data;
using SupportService.Consumers;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

// SupportDb sisteme kayýt.
builder.Services.AddDbContext<SupportDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//Consumer için MassTransit ve RabbitMQ Ayarlarý
builder.Services.AddMassTransit(x =>
{
    // Dinleyiciyi sisteme tanýtma
    x.AddConsumer<SlaWarningConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        //Hangi kuyruk dinlenecek
        cfg.ReceiveEndpoint("sla-warning-queue", e =>
        {
            e.ConfigureConsumer<SlaWarningConsumer>(context);
        });
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
//WebSocket , Long Polling Fallback
app.MapHub<SupportService.Hubs.SupportChatHub>("/support-chat", options =>
{
    // Sadece WebSockets ve LongPolling'e izin var. ServerSentEvents iptal.
    options.Transports =
        Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
        Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
});

app.Run();
public partial class Program { }