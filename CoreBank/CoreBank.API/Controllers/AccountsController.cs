using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreBank.API.Data;
using CoreBank.API.DTOs;
using System.Security.Claims;

namespace CoreBank.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("my-accounts")]
        public async Task<IActionResult> GetMyAccounts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Geçersiz token kimliği." });
            }

            var accounts = await _context.Accounts
                .Where(a => a.KullaniciId == userId)
                .Select(a => new AccountDto
                {
                    HesapId = a.HesapId,
                    HesapIban = a.HesapIban,
                    HesapBakiye = a.HesapBakiye,
                    HesapParaBirimi = a.HesapParaBirimi
                })
                .ToListAsync();

            return Ok(accounts);
        }
    }
}
