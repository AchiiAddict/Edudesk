using MassTransit;
using WorkerService.Events;

namespace WorkerService.Workers
{
    // IConsumer arayüzü MassTransitten gelir, Bu sınıf RabbitMQ'yu dinler.
    public class CertificateConsumer : IConsumer<CertificateRequestedEvent>
    {
        private readonly ILogger<CertificateConsumer> _logger;

        public CertificateConsumer(ILogger<CertificateConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CertificateRequestedEvent> context)
        {
            var data = context.Message;
            _logger.LogInformation("{StudentName} adlı öğrenci için {CourseName} sertifikası hazırlanıyor.", data.StudentName, data.CourseName);

            await Task.Delay(3000); 

            _logger.LogInformation("Sertifika PDFi başarıyla üretildi.");

        }
    }
}