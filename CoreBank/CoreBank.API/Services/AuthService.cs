using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoreBank.API.Data;
using CoreBank.API.DTOs;
using CoreBank.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreBank.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.KullaniciEposta == dto.Eposta);
            if (existingUser != null)
            {
                return (false, "Bu e-posta adresi ile zaten kayıtlı bir kullanıcı var.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Sifre);

            var newUser = new User
            {
                KullaniciAdSoyad = dto.AdSoyad,
                KullaniciTc = dto.Tc,
                KullaniciEposta = dto.Eposta,
                KullaniciSifre = passwordHash,
                OlusturmaTarihi = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Yeni kullanıcıya otomatik IBAN ve bakiye tanımlama
            var newAccount = new Account
            {
                KullaniciId = newUser.KullaniciId,
                HesapIban = "TR" + Guid.NewGuid().ToString("N")[..24].ToUpper(),
                HesapBakiye = 1000m,
                HesapParaBirimi = "TRY"
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return (true, "Kayıt başarıyla oluşturuldu.");
        }

        public async Task<(bool Success, string Message, string? Token, string? AdSoyad, string? Iban)> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.KullaniciEposta == dto.Eposta);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Sifre, user.KullaniciSifre))
            {
                return (false, "E-posta veya şifre hatalı.", null, null, null);
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.KullaniciId == user.KullaniciId);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.KullaniciId.ToString()),
                    new Claim(ClaimTypes.Email, user.KullaniciEposta),
                    new Claim(ClaimTypes.Name, user.KullaniciAdSoyad)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return (true, "Giriş başarılı.", tokenString, user.KullaniciAdSoyad, account?.HesapIban);
        }
    }
}