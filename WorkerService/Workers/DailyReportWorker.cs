using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerService.Workers
{
    public class DailyReportWorker : BackgroundService
    {
        private readonly ILogger<DailyReportWorker> _logger;

        public DailyReportWorker(ILogger<DailyReportWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Günlük Rapor Amele göreve başladı...");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Günlük Raporcu: Tüm tenantlar için günlük istatistikler toplanıyor ve e-posta kuyruğuna iletiliyor...");

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}