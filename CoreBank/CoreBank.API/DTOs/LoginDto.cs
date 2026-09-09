using System.ComponentModel.DataAnnotations;

namespace CoreBank.API.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        public string Sifre { get; set; } = string.Empty;
    }
}
