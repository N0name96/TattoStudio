namespace TattoStudio.Domain.Enums;

/// <summary>
/// Rol del staff. Mapeado como smallint en Postgres (spec REG-01 §3).
/// </summary>
public enum Role
{
    Admin     = 0,
    Recepcion = 1,
    Artista   = 2
}
