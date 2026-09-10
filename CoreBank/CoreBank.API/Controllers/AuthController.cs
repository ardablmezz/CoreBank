using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreBank.API.Data;
using CoreBank.API.Models;
using CoreBank.API.DTOs;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CoreBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.KullaniciTc == dto.Tc || u.KullaniciEposta == dto.Eposta);
            if (userExists)
            {
                return BadRequest(new { message = "Bu T.C. Kimlik No veya E-posta adresi ile kayıtlı kullanıcı zaten mevcut." });
            }

            var newUser = new User
            {
                KullaniciAdSoyad = dto.AdSoyad,
                KullaniciTc = dto.Tc,
                KullaniciEposta = dto.Eposta,
                KullaniciSifre = BCrypt.Net.BCrypt.HashPassword(dto.Sifre),
                OlusturmaTarihi = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var random = new Random();
            string part1 = random.Next(10000000, 99999999).ToString();
            string part2 = random.Next(10000000, 99999999).ToString();
            string part3 = random.Next(10000000, 99999999).ToString();
            var defaultAccount = new Account
            {
                KullaniciId = newUser.KullaniciId,
                HesapIban = "TR" + part1 + part2 + part3,
                HesapParaBirimi = "TRY",
                HesapBakiye = 1000.00m,
                OlusturmaTarihi = DateTime.UtcNow
            };

            _context.Accounts.Add(defaultAccount);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Kayıt işlemi başarılı! Hesabınız oluşturuldu.",
                userId = newUser.KullaniciId,
                iban = defaultAccount.HesapIban,
                bakiye = defaultAccount.HesapBakiye
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Accounts)
                .FirstOrDefaultAsync(u => u.KullaniciEposta == dto.Eposta);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Sifre, user.KullaniciSifre))
            {
                return Unauthorized(new { message = "E-posta veya şifre hatalı." });
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                message = "Giriş başarılı.",
                token = token,
                userId = user.KullaniciId,
                adSoyad = user.KullaniciAdSoyad,
                eposta = user.KullaniciEposta,
                hesaplar = user.Accounts.Select(a => new
                {
                    a.HesapId,
                    a.HesapIban,
                    a.HesapBakiye,
                    a.HesapParaBirimi
                })
            });
        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.KullaniciId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.KullaniciEposta),
                new Claim("FullName", user.KullaniciAdSoyad),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
