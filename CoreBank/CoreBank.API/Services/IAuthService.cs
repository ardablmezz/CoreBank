using CoreBank.API.DTOs;

namespace CoreBank.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string Message, string? Token, string? AdSoyad, string? Iban)> LoginAsync(LoginDto dto);
    }
}
