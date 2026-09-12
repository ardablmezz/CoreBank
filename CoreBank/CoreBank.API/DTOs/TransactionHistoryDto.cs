namespace CoreBank.API.DTOs
{
    public class TransactionHistoryDto
    {
        public int IslemId { get; set; }
        public string GonderenIban { get; set; } = string.Empty;
        public string AliciIban { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string IslemTipi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
    }
}
