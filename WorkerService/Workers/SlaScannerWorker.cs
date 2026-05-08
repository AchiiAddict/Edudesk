using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerService.Workers
{
    public class SlaScannerWorker : BackgroundService
    {
        private readonly ILogger<SlaScannerWorker> _logger;

        public SlaScannerWorker(ILogger<SlaScannerWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SLA Tarayıcı Worker görve başladı");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Veritabanındaki açık biletler taranıyor. Tarih: {time}", DateTimeOffset.Now);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}