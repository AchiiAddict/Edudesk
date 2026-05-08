using IdentityService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný þalteri
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// API ve Swagger (Dokümantasyon) destekleri
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Swagger arayüzünü ayaða kaldýrma.
app.UseSwagger();
app.UseSwaggerUI();

//Gelen istekleri ilgili Controller'lara yönlendirme
app.MapControllers();

app.Run();