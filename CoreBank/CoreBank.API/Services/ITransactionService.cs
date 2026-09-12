using CoreBank.API.DTOs;

namespace CoreBank.API.Services
{
    public interface ITransactionService
    {
        Task<(bool Success, string Message)> TransferAsync(int senderUserId, TransferDto dto);
        Task<(bool Success, string Message, List<TransactionHistoryDto>? History)> GetHistoryAsync(int userId, string iban);
    }
}