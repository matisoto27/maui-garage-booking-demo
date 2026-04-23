namespace Demo.Shared.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Dni { get; set; } = string.Empty;

    public string Contrasena { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;
}
