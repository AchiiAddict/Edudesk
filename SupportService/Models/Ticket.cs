namespace SupportService.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; } // bilet hangi kurumda
        public Guid UserId { get; set; } // bileti kim açtı

        public string Subject { get; set; }
        public string Description { get; set; }
        public bool IsResolved { get; set; } = false;

        // Aciliyet parametresi 1/2/3, low/med/high
        public int Urgency { get; set; }

        // Kiracı katsayısı 1/2/3, basic/pro/enterprise
        public int PlanMultiplier { get; set; }

        // SLA Bitiş Tarihi
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime SlaDeadline { get; set; }

        //Idempotency Key
        public string? IdempotencyKey { get; set; }

        // Bracket Score anlık olarak hesaplanır, veri tabanıında sütün olarak tutulmaz
        public double BracketScore
        {
            get
            {
                if (IsResolved) return 0; // çıkış

                // saat cinsinden kalan süre hesaplama
                double remainingSlaHours = (SlaDeadline - DateTime.UtcNow).TotalHours;

                // SLA ihlali kontrolü
                if (remainingSlaHours <= 0) remainingSlaHours = 0.1;

                //aciliyet × kalan SLA süresi × kiracı plan katsayısı (istenen durum)
                return Urgency * remainingSlaHours * PlanMultiplier;
            }
        }
    }
}