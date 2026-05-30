namespace TattoStudio.Domain.Entities
{
    public class Appoinment
    {
        public Guid Id { get; set; }
        public Guid ArtistId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MailClient { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public byte[]? TattoImage { get; set; }
        public bool Deposit { get; set; } = false;
        public decimal DepositAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime AppoinmentDate { get; set; }
        public bool SignedConsentForm { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}