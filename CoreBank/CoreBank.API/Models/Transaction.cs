using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreBank.API.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        [Column("islem_id")]
        public int IslemId { get; set; }

        [Required]
        [Column("gonderen_hesap_id")]
        public int GonderenHesapId { get; set; }

        [Required]
        [Column("alici_hesap_id")]
        public int AliciHesapId { get; set; }

        [Required]
        [Column("islem_tutar", TypeName = "decimal(18,2)")]
        public decimal IslemTutar { get; set; }

        [MaxLength(200)]
        [Column("islem_aciklama")]
        public string? IslemAciklama { get; set; }

        [Column("islem_tarihi")]
        public DateTime? IslemTarihi { get; set; } = DateTime.UtcNow;

        [ForeignKey("GonderenHesapId")]
        public Account? GonderenHesap { get; set; }

        [ForeignKey("AliciHesapId")]
        public Account? AliciHesap { get; set; }
    }
}
