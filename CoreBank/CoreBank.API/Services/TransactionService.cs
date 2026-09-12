using CoreBank.API.Data;
using CoreBank.API.DTOs;
using CoreBank.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreBank.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> TransferAsync(int senderUserId, TransferDto dto)
        {
            if (dto.GonderenIban == dto.AliciIban)
            {
                return (false, "Kendinize ait aynı hesaba transfer yapamazsınız.");
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var gonderenHesap = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.HesapIban == dto.GonderenIban);

                if (gonderenHesap == null)
                {
                    return (false, "Gönderici hesap bulunamadı.");
                }

                if (gonderenHesap.KullaniciId != senderUserId)
                {
                    return (false, "Bu hesaptan para gönderme yetkiniz yok.");
                }

                if (gonderenHesap.HesapBakiye < dto.Tutar)
                {
                    return (false, "Yetersiz bakiye.");
                }

                var aliciHesap = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.HesapIban == dto.AliciIban);

                if (aliciHesap == null)
                {
                    return (false, "Alıcı hesap bulunamadı.");
                }

                gonderenHesap.HesapBakiye -= dto.Tutar;
                aliciHesap.HesapBakiye += dto.Tutar;

                var transactionRecord = new Transaction
                {
                    GonderenHesapId = gonderenHesap.HesapId,
                    AliciHesapId = aliciHesap.HesapId,
                    IslemTutar = dto.Tutar,
                    IslemAciklama = dto.Aciklama,
                    IslemTarihi = DateTime.UtcNow
                };

                _context.Transactions.Add(transactionRecord);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return (true, "Transfer işlemi başarıyla tamamlandı.");
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<(bool Success, string Message, List<TransactionHistoryDto>? History)> GetHistoryAsync(int userId, string iban)
        {
            var targetAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.HesapIban == iban);
            if (targetAccount == null)
            {
                return (false, "Hesap bulunamadı.", null);
            }

            if (targetAccount.KullaniciId != userId)
            {
                return (false, "Bu hesaba ait hareketleri görüntüleme yetkiniz yok.", null);
            }

            var transactions = await _context.Transactions
                .Include(t => t.GonderenHesap)
                .Include(t => t.AliciHesap)
                .Where(t => t.GonderenHesapId == targetAccount.HesapId || t.AliciHesapId == targetAccount.HesapId)
                .OrderByDescending(t => t.IslemTarihi)
                .Select(t => new TransactionHistoryDto
                {
                    IslemId = t.IslemId,
                    GonderenIban = t.GonderenHesap != null ? t.GonderenHesap.HesapIban : string.Empty,
                    AliciIban = t.AliciHesap != null ? t.AliciHesap.HesapIban : string.Empty,
                    Tutar = t.IslemTutar,
                    IslemTarihi = t.IslemTarihi ?? DateTime.UtcNow,
                    IslemTipi = t.GonderenHesapId == targetAccount.HesapId ? "GIDEN" : "GELEN",
                    Aciklama = t.IslemAciklama
                })
                .ToListAsync();

            return (true, "İşlem hareketleri başarıyla getirildi.", transactions);
        }
    }
}