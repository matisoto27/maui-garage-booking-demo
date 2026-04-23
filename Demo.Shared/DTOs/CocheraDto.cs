namespace Demo.Shared.Dtos;

public class CocheraDto
{
    public int IdCochera { get; set; }

    public string Direccion { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public double Latitud { get; set; }

    public double Longitud { get; set; }
}
