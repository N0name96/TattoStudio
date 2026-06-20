namespace TattoStudio.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa a un cliente del estudio de tatuajes.
/// </summary>
public sealed class Client
{
    private Client() { }

    public Client(string name, string phone, string email, DateOnly birthDate, string? medicalNotes)
    {
        Id           = Guid.NewGuid();
        Name         = name;
        Phone        = phone;
        Email        = email;
        BirthDate    = birthDate;
        MedicalNotes = medicalNotes;
    }

    public Guid     Id           { get; private set; }
    public string   Name         { get; private set; } = null!;
    public string   Phone        { get; private set; } = null!;
    public string   Email        { get; private set; } = null!;
    public DateOnly BirthDate    { get; private set; }
    public string?  MedicalNotes { get; private set; }
}
