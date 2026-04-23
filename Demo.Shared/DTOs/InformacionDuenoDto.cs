namespace Demo.Shared.Dtos;

public class InformacionDuenoDto
{
    public int IdCochera { get; set; }

    public string Dni { get; set; } = string.Empty;

    public string ApellidoNombre { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string DireccionCompleta { get; set; } = string.Empty;

    public string EntreCalles { get; set; } = string.Empty;

    public string Precio { get; set; } = string.Empty;

    public string PuntuacionDueno { get; set; } = string.Empty;

    public string PuntuacionCochera { get; set; } = string.Empty;

    public bool MostrarTextoSinCalificacionesDeOtrosDueno { get; set; }

    public bool MostrarVerMasDueno { get; set; }

    public bool MostrarTextoSinCalificacionesDeOtrosCochera { get; set; }

    public bool MostrarVerMasCochera { get; set; }
}
