namespace WorkerService.Events
{
    // Bu bizim mektubumuz!
    public class SlaWarningEvent
    {
        public Guid TicketId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}