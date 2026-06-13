namespace TattoStudio.Domain.Entities;

public class Artist
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public decimal Comision { get; set; }
    public Guid UserId { get; set; }
}
