using Demo.Api.Data;
using Demo.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers;

[ApiController]
[Route("api")]
public class UsuarioController : ControllerBase
{
    private readonly FakeDataStore _dataStore;

    public UsuarioController(FakeDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    [HttpPost("auth/login")]
    public ActionResult<IniciarSesionResponseDto> IniciarSesion([FromBody] IniciarSesionRequestDto request)
    {
        var usuario = _dataStore.Usuarios.FirstOrDefault(u => u.Dni == request.Dni && u.Contrasena == request.Contrasena);
        if (usuario is null)
        {
            return Ok(new IniciarSesionResponseDto
            {
                Exito = false,
                Mensaje = "DNI o contraseña incorrectos."
            });
        }

        return Ok(new IniciarSesionResponseDto
        {
            Exito = true,
            Mensaje = "Inicio de sesión correcto.",
            IdUsuario = usuario.Id
        });
    }

    [HttpGet("usuarios/{idUsuario:int}/cocheras/cantidad")]
    public ActionResult<int> ObtenerCantidadCocheras(int idUsuario)
    {
        int cantidad = _dataStore.Cocheras.Count(c => c.UsuarioId == idUsuario);
        return Ok(cantidad);
    }

    [HttpGet("cocheras/mapa")]
    public ActionResult<List<CocheraDto>> ObtenerCocherasMapa()
    {
        var resultado = _dataStore.Cocheras
            .Select(c => new CocheraDto
            {
                IdCochera = c.Id,
                Direccion = $"{c.CallePrincipal} {c.Altura}",
                Precio = c.Precio,
                Latitud = c.Latitud,
                Longitud = c.Longitud
            })
            .ToList();

        return Ok(resultado);
    }

    [HttpGet("cocheras/{idCochera:int}/info-dueno")]
    public ActionResult<InformacionDuenoDto> ObtenerInformacionDueno(int idCochera)
    {
        var cochera = _dataStore.Cocheras.FirstOrDefault(c => c.Id == idCochera);
        if (cochera is null)
        {
            return NotFound();
        }

        var dueno = _dataStore.Usuarios.FirstOrDefault(u => u.Id == cochera.UsuarioId);
        if (dueno is null)
        {
            return NotFound();
        }

        return Ok(new InformacionDuenoDto
        {
            IdCochera = cochera.Id,
            Dni = dueno.Dni,
            ApellidoNombre = $"{dueno.Apellido}, {dueno.Nombre}",
            Telefono = dueno.Telefono,
            DireccionCompleta = $"{cochera.CallePrincipal} {cochera.Altura}",
            EntreCalles = $"Entre {cochera.EntreCallePrimera} y {cochera.EntreCalleSegunda}",
            Precio = $"${cochera.Precio} por hora",
            PuntuacionDueno = "12 puntuaciones\nPromedio 4.7 / 5.0",
            PuntuacionCochera = "9 puntuaciones\nPromedio 4.5 / 5.0",
            MostrarTextoSinCalificacionesDeOtrosDueno = false,
            MostrarVerMasDueno = true,
            MostrarTextoSinCalificacionesDeOtrosCochera = false,
            MostrarVerMasCochera = true
        });
    }

    [HttpGet("cocheras/{idCochera:int}/resumen-reserva")]
    public ActionResult<ResumenReservaDto> ObtenerResumenReserva(int idCochera)
    {
        var cochera = _dataStore.Cocheras.FirstOrDefault(c => c.Id == idCochera);
        if (cochera is null)
        {
            return NotFound();
        }

        return Ok(new ResumenReservaDto
        {
            IdCochera = cochera.Id,
            DireccionCompleta = $"{cochera.CallePrincipal} {cochera.Altura}",
            EntreCalles = $"Entre {cochera.EntreCallePrimera} y {cochera.EntreCalleSegunda}",
            PrecioPorHora = cochera.Precio
        });
    }
}
