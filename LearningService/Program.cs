using LearningService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsý
builder.Services.AddDbContext<LearningDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
// WebSocket , Long Polling Fallback Ayarý
app.MapHub<LearningService.Hubs.LiveLessonHub>("/live-lesson", options =>
{
    options.Transports =
        Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
        Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
});
app.Run();