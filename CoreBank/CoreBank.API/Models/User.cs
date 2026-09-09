using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreBank.API.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("kullanici_id")]
        public int KullaniciId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("kullanici_adsoyad")]
        public string KullaniciAdSoyad { get; set; } = string.Empty;

        [Required]
        [StringLength(11, MinimumLength = 11)]
        [Column("kullanici_tc")]
        public string KullaniciTc { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        [Column("kullanici_eposta")]
        public string KullaniciEposta { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("kullanici_sifre")]
        public string KullaniciSifre { get; set; } = string.Empty;

        [Column("kullanici_olusturulmatarihi")]
        public DateTime? OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
