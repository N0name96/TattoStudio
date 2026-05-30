namespace TattoStudio.Application.DTOs.Appoinments
{
    public record UpdateAppoinmentRequest
    {
        public Guid? ArtistId { get; init; }
        public string? Name { get; init; }
        public string? MailClient { get; init; }
        public string? PhoneNumber { get; init; }
        public byte[]? TattoImage { get; init; }
        public bool? Deposit { get; init; } = false;
        public decimal? DepositAmount { get; init; }
        public decimal? TotalPrice { get; init; }
        public DateTime? AppoinmentDate { get; init; }
        public bool? SignedConsentForm { get; init; }
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
