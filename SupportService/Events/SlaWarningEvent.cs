namespace WorkerService.Events
{
    public class SlaWarningEvent
    {
        public Guid TicketId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}