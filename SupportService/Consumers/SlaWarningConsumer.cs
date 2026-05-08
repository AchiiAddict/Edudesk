using MassTransit;
using WorkerService.Events;

namespace SupportService.Consumers
{
    
    public class SlaWarningConsumer : IConsumer<SlaWarningEvent>
    {
        private readonly ILogger<SlaWarningConsumer> _logger;

        public SlaWarningConsumer(ILogger<SlaWarningConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<SlaWarningEvent> context)
        {
            var mektup = context.Message;

            //test logu basıyoruz.

            _logger.LogWarning("RABBITMQ Mesaj :  Bilet ID: {TicketId}, Konu: {Subject}, Uyarı: {Message}",
                mektup.TicketId, mektup.Subject, mektup.Message);

            return Task.CompletedTask;
        }
    }
}