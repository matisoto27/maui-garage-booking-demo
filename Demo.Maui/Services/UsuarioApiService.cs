using System.Net.Http.Json;
using Demo.Shared.Dtos;

namespace Demo.Maui.Services;

public class UsuarioApiService
{
    private readonly HttpClient _httpClient;

    public UsuarioApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IniciarSesionResponseDto> IniciarSesionAsync(string dni, string contrasena)
    {
        IniciarSesionRequestDto request = new()
        {
            Dni = dni,
            Contrasena = contrasena
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
        {
            return new IniciarSesionResponseDto
            {
                Exito = false,
                Mensaje = "No se pudo conectar con el servidor."
            };
        }

        IniciarSesionResponseDto? loginResponse = await response.Content.ReadFromJsonAsync<IniciarSesionResponseDto>();
        return loginResponse ?? new IniciarSesionResponseDto
        {
            Exito = false,
            Mensaje = "Respuesta inválida del servidor."
        };
    }

    public async Task<int> ObtenerCantidadCocherasAsync(int idUsuario)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/usuarios/{idUsuario}/cocheras/cantidad");
        if (!response.IsSuccessStatusCode)
        {
            return 0;
        }

        int? cantidad = await response.Content.ReadFromJsonAsync<int>();
        return cantidad ?? 0;
    }

    public async Task<List<CocheraDto>> ObtenerCocherasMapaAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("api/cocheras/mapa");
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        List<CocheraDto>? cocheras = await response.Content.ReadFromJsonAsync<List<CocheraDto>>();
        return cocheras ?? [];
    }

    public async Task<InformacionDuenoDto?> ObtenerInformacionDuenoAsync(int idCochera)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/cocheras/{idCochera}/info-dueno");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<InformacionDuenoDto>();
    }

    public async Task<ResumenReservaDto?> ObtenerResumenReservaAsync(int idCochera)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/cocheras/{idCochera}/resumen-reserva");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ResumenReservaDto>();
    }
}
