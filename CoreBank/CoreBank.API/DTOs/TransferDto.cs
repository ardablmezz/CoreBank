using System.ComponentModel.DataAnnotations;

namespace CoreBank.API.DTOs
{
    public class TransferDto
    {
        [Required(ErrorMessage = "Gönderen hesap IBAN'ı zorunludur.")]
        [StringLength(26, MinimumLength = 26, ErrorMessage = "IBAN 26 haneli olmalıdır.")]
        public string GonderenIban { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alıcı hesap IBAN'ı zorunludur.")]
        [StringLength(26, MinimumLength = 26, ErrorMessage = "IBAN 26 haneli olmalıdır.")]
        public string AliciIban { get; set; } = string.Empty;

        [Required(ErrorMessage = "Transfer tutarı zorunludur.")]
        [Range(1.00, 1000000.00, ErrorMessage = "Transfer tutarı en az 1 TL olmalıdır.")]
        public decimal Tutar { get; set; }

        [MaxLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
        public string? Aciklama { get; set; }
    }
}
