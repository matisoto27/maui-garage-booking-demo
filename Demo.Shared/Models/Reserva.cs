namespace Demo.Shared.Models;

public class Reserva
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int CocheraId { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }
}
