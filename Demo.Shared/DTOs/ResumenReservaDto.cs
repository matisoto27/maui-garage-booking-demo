namespace Demo.Shared.Dtos;

public class ResumenReservaDto
{
    public int IdCochera { get; set; }

    public string DireccionCompleta { get; set; } = string.Empty;

    public string EntreCalles { get; set; } = string.Empty;

    public decimal PrecioPorHora { get; set; }
}
