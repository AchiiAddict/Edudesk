using Microsoft.AspNetCore.Mvc;
using SupportService.Data;
using SupportService.Models;

namespace SupportService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly SupportDbContext _context;

        public TicketController(SupportDbContext context)
        {
            _context = context;
        }

        // POST: api/ticket , yeni bilet
        [HttpPost]
        public IActionResult CreateTicket([FromBody] Ticket newTicket, [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
        {
            // Mülakat Kuralı: Idempotency Key kontrolü (Çift bilet açılmasını engeller)
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                return BadRequest("Güvenlik sebebiyle 'Idempotency-Key' header'ı zorunludur!");
            }

            // Gelen key veritabanında var mı kontrol et
            if (_context.Tickets.Any(t => t.IdempotencyKey == idempotencyKey))
            {
                // Aynı key daha önce gelmişse, tekrar kaydetme ama hata da fırlatma (Idempotent mantığı)
                return Ok(new { Mesaj = "Bu bilet daha önce açılmış (Idempotency koruması)." });
            }

            newTicket.Id = Guid.NewGuid();
            newTicket.CreatedAt = DateTime.UtcNow;
            newTicket.IdempotencyKey = idempotencyKey;

            // Döküman Kuralı: SLA süreleri tenant planına göre atanır
            // PlanMultiplier: 1 (Basic), 2 (Pro), 3 (Enterprise)
            int slaHours = newTicket.PlanMultiplier switch
            {
                3 => 2,   // Enterprise: 2 saat
                2 => 8,   // Pro: 8 saat
                1 => 48,  // Basic: 48 saat
                _ => 48   // Varsayılan
            };

            newTicket.SlaDeadline = DateTime.UtcNow.AddHours(slaHours);

            _context.Tickets.Add(newTicket);
            _context.SaveChanges();

            return Ok(new { Mesaj = "Bilet başarıyla açıldı!", Data = newTicket });
        }

        // GET: api/ticket/queue
        [HttpGet("queue")]
        public IActionResult GetSupportQueue()
        {
            // Bracket Score için sütun oluşturmadık. Önce veriyi RAM'e çekip sonra c# üzerinde sıralama yapıyoruz.

            var queue = _context.Tickets
                .Where(t => t.IsResolved == false) // çözülmemiş biletleri getir.
                .ToList() // veriyi RAM'e al
                .OrderBy(t => t.BracketScore) // küçükten büyüğe sort işlemi
                .Select(t => new
                {
                    t.Id,
                    t.Subject,
                    t.Urgency,
                    t.PlanMultiplier,
                    KalanSlaSaat = Math.Round((t.SlaDeadline - DateTime.UtcNow).TotalHours, 1),
                    Skor = Math.Round(t.BracketScore, 2)
                })
                .ToList();

            return Ok(queue);
        }
    }
}