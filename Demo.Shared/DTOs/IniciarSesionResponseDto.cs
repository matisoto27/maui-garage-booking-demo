namespace Demo.Shared.Dtos;

public class IniciarSesionResponseDto
{
    public bool Exito { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public int IdUsuario { get; set; }
}
