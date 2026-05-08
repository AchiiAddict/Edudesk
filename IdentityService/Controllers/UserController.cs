using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.Data;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        // POST: api/user/register
        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] User newUser)
        {
            // Kurum kontrolü, tenant varlığı kontrol ediliyor.
            var tenantExists = _context.Tenants.Any(t => t.Id == newUser.TenantId);
            if (!tenantExists)
            {
                return BadRequest("Hata: Belirtilen kurum (Tenant) bulunamadı!");
            }

            // Bycrypt ile şifreleme.
            // Kullanıcının girdiği düz şifreyi alıp, Hash'liyoruz.
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);

            // Kullanıcıya yeni bir ID veriyoruz
            newUser.Id = Guid.NewGuid();

            //VeriTabanına kayıt.
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Güvenlik için şifre gizleme
            newUser.PasswordHash = "******";

            return Ok(new { Mesaj = "Kullanıcı başarıyla kaydedildi!", Data = newUser });
        }

        // POST: api/user/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Kullanıcı mail kontrol
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null) return Unauthorized("Kullanıcı bulunamadı.");

            // Şifre doğrulaması BYCRYPT ile
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid) return Unauthorized("Şifre hatalı.");

            // JWT TOKEN üretimi. 
            var tokenHandler = new JwtSecurityTokenHandler();
            // appsettings.json'da key çekme işlemi
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

            // Token'ın içine koyacağımız bilgiler.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("TenantId", user.TenantId.ToString()), // kurum
                    new Claim(ClaimTypes.Role, user.Role) //rol
                }),
                // Access token 15 dakika boyunca geçerliliğini koruyacak.
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Döküman Kuralı: Refresh Token (7 gün)
            var refreshToken = Guid.NewGuid().ToString();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            return Ok(new
            {
                Token = tokenString,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration,
                Mesaj = "Giriş başarılı!"
            });
        }
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}