using System.ComponentModel.DataAnnotations;

namespace CoreBank.API.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        [MaxLength(100)]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "T.C. Kimlik Numarası zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "T.C. Kimlik No 11 haneli olmalıdır.")]
        public string Tc { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Sifre { get; set; } = string.Empty;
    }
}
