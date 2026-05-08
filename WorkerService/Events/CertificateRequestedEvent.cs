namespace WorkerService.Events
{
    public class CertificateRequestedEvent
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
    }
}