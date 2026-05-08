namespace IdentityService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; } // Row-Level Veri İzolasyonu kuralı için Tenant bağlantısı
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // "Şifreler düz metin saklanamaz" kuralıı
        public string Role { get; set; } //Admin, Instructor, Student, Support
    }
}