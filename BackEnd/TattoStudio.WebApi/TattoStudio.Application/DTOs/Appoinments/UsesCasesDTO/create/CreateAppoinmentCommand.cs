using MediatR;

namespace TattoStudio.Application.DTOs.Appoinments
{
    public record CreateAppoinmentCommand : IRequest<AppoinmentDTO>
    {
        public Guid ArtistId { get; init; }
        public string Name { get; init; }
        public string MailClient { get; init; }
        public string? PhoneNumber { get; init; }
        public byte[]? TattoImage { get; init; }
        public bool Deposit { get; init; } = false;
        public decimal DepositAmount { get; init; }
        public decimal TotalPrice { get; init; }
        public DateTime AppoinmentDate { get; init; }
        public bool SignedConsentForm { get; init; } = false;
    }
}
