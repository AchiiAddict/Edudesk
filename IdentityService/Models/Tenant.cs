namespace IdentityService.Models
{
    public class Tenant
    {
        public Guid Id { get; set; } // Benzersiz kimlik
        public string Name { get; set; } // Kurum Adı (Örn: X Üniversitesi)
        public string Plan { get; set; } // Dökümandaki SLA planları: Basic, Pro, Enterprise
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}