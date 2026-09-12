namespace CoreBank.API.DTOs
{
    public class AccountDto
    {
        public int HesapId { get; set; }
        public string HesapIban { get; set; } = string.Empty;
        public decimal HesapBakiye { get; set; }
        public string HesapParaBirimi { get; set; } = string.Empty;
    }
}
