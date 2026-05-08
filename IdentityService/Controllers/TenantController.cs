using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Dependency Injection: AppDbContext'e erişim.
        public TenantController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/tenant yeni kurum kayıdı
        [HttpPost]
        public IActionResult CreateTenant([FromBody] Tenant newTenant)
        {
            // Yeni kuruma rastgele benzersiz bir ID atama işlemi
            newTenant.Id = Guid.NewGuid();
            newTenant.CreatedAt = DateTime.UtcNow;

            // Veritabanına ekle ve kaydet
            _context.Tenants.Add(newTenant);
            _context.SaveChanges();

            return Ok(new { Mesaj = "Kurum başarıyla kaydedildi!", Data = newTenant });
        }

        // GET: api/tenant,kayıtlı kurum listesini get ediyoruz.
        [HttpGet]
        public IActionResult GetTenants()
        {
            var tenants = _context.Tenants.ToList();
            return Ok(tenants);
        }
    }
}