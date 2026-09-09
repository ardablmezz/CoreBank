using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreBank.API.Data;
using CoreBank.API.Models;
using CoreBank.API.DTOs;

namespace CoreBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
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
                KullaniciSifre = dto.Sifre,
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
                .FirstOrDefaultAsync(u => u.KullaniciEposta == dto.Eposta && u.KullaniciSifre == dto.Sifre);

            if (user == null)
            {
                return Unauthorized(new { message = "E-posta veya şifre hatalı." });
            }

            return Ok(new
            {
                message = "Giriş başarılı.",
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
    }
}
