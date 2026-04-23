namespace Demo.Shared.Models;

public class Cochera
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string CallePrincipal { get; set; } = string.Empty;

    public int Altura { get; set; }

    public string EntreCallePrimera { get; set; } = string.Empty;

    public string EntreCalleSegunda { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public double Latitud { get; set; }

    public double Longitud { get; set; }
}
