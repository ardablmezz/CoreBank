using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoreBank.API.Data;
using CoreBank.API.Models;
using CoreBank.API.DTOs;

namespace CoreBank.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController: ControllerBase
    {
        private readonly AppDbContext _context;
        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            if (dto.GonderenIban == dto.AliciIban)
            {
                return BadRequest(new { message = "Kendi hesabınıza aynı IBAN üzerinden transfer yapamazsınız." });
            }

            var senderAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.HesapIban == dto.GonderenIban);
            if (senderAccount == null)
            {
                return NotFound(new { message = "Gönderen IBAN sistemde bulunamadı." });
            }

            var receiverAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.HesapIban == dto.AliciIban);
            if (receiverAccount == null)
            {
                return NotFound(new { message = "Alıcı IBAN sistemde bulunamadı." });
            }

            if (senderAccount.HesapBakiye < dto.Tutar)
            {
                return BadRequest(new { message = "Yetersiz bakiye. Mevcut bakiye: " + senderAccount.HesapBakiye });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                senderAccount.HesapBakiye -= dto.Tutar;
                receiverAccount.HesapBakiye += dto.Tutar;

                var newTransaction = new Transaction
                {
                    GonderenHesapId = senderAccount.HesapId,
                    AliciHesapId = receiverAccount.HesapId,
                    IslemTutar = dto.Tutar,
                    IslemAciklama = dto.Aciklama ?? "Hesaplar arası havale",
                    IslemTarihi = DateTime.UtcNow
                };

                _context.Transactions.Add(newTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Transfer işlemi başarıyla gerçekleşti.",
                    transactionId = newTransaction.IslemId,
                    gonderenKalanBakiye = senderAccount.HesapBakiye,
                    transferTutari = dto.Tutar,
                    tarih = newTransaction.IslemTarihi
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Transfer sırasında bir hata oluştu.", detail = ex.Message });
            }
        }
    }
}
