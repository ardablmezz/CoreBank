using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreBank.API.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        [Column("hesap_id")]
        public int HesapId { get; set; }

        [Required]
        [Column("kullanici_id")]
        public int KullaniciId { get; set; }

        [Required]
        [StringLength(26, MinimumLength = 26)]
        [Column("hesap_iban")]
        public string HesapIban { get; set; } = string.Empty;

        [Required]
        [StringLength(3)]
        [Column("hesap_parabirimi")]
        public string HesapParaBirimi { get; set; } = "TRY";

        [Column("hesap_bakiye", TypeName = "decimal(18,2)")]
        public decimal HesapBakiye { get; set; } = 0.00m;

        [Column("hesap_olusturulmatarihi")]
        public DateTime? OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        [ForeignKey("KullaniciId")]
        public User? User { get; set; }

        [InverseProperty("GonderenHesap")]
        public ICollection<Transaction> GonderilenIslemler { get; set; } = new List<Transaction>();

        [InverseProperty("AliciHesap")]
        public ICollection<Transaction> AlinanIslemler { get; set; } = new List<Transaction>();
    }
}
