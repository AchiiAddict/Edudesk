using MassTransit;
using WorkerService.Events;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBus _bus; // MassTransit

        public Worker(ILogger<Worker> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SLA Zamanlayýcýsý baþladý.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("SLA Taramasý yapýlýyor.");

    
                // bir biletin süresi dolmuþ gibi event atýyoruz

                var uyariMektubu = new SlaWarningEvent
                {
                    TicketId = Guid.NewGuid(),
                    Subject = "Kritik Þifre Sýfýrlama Talebi",
                    Message = "Bu biletin SLA süresi aþýlmak üzere."
                };

                // RabbitMQ gönder.
                await _bus.Publish(uyariMektubu, stoppingToken);

                _logger.LogWarning("SlaWarningEvent gönderildi.");

                await Task.Delay(60000, stoppingToken); // 1 dakika bekle
            }
        }
    }
}