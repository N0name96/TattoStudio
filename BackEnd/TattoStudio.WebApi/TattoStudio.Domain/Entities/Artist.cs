namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa a un artista del estudio de tatuajes.
/// </summary>
public sealed class Artist
{
    private Artist() { }

    public Artist(string name, string specialty, decimal commissionPercentage)
    {
        Id                   = Guid.NewGuid();
        Name                 = name;
        Specialty            = specialty;
        CommissionPercentage = commissionPercentage;
    }

    public Guid    Id                   { get; private set; }
    public string  Name                 { get; private set; } = null!;
    public string  Specialty            { get; private set; } = null!;
    public decimal CommissionPercentage { get; private set; }
}
