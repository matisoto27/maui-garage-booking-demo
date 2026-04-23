using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo.Maui.Services;
using Demo.Shared.Dtos;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Maps;

namespace Demo.Maui.ViewModels;

public partial class MapaViewModel : ObservableObject
{
    private readonly UsuarioApiService _usuarioApiService;
    private CocheraDto? _cocheraSeleccionada;

    [ObservableProperty]
    private bool estaCargando;

    [ObservableProperty]
    private string direccion = "Seleccione una cochera en el mapa";

    public Location CentroMapa { get; } = new(-32.944390, -60.650500);

    public Location UbicacionCochera { get; } = new(-32.943950, -60.651200);

    public MapaViewModel(UsuarioApiService usuarioApiService)
    {
        _usuarioApiService = usuarioApiService;
    }

    [RelayCommand]
    private async Task Volver()
    {
        await Shell.Current.GoToAsync("//Inicio");
    }

    [RelayCommand]
    private async Task Siguiente()
    {
        if (_cocheraSeleccionada == null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"//InformacionDueno?idCochera={_cocheraSeleccionada.IdCochera}");
    }

    public async Task<List<CocheraDto>> ObtenerCocherasAsync()
    {
        return await _usuarioApiService.ObtenerCocherasMapaAsync();
    }

    public async Task<bool> UsuarioPuedeUsarMapaComoClienteAsync()
    {
        int cantidad = await _usuarioApiService.ObtenerCantidadCocherasAsync(SesionDemoContext.IdUsuario);
        return cantidad == 0;
    }

    public void SeleccionarCochera(CocheraDto cochera)
    {
        _cocheraSeleccionada = cochera;
        Direccion = $"Dirección: {cochera.Direccion}";
    }
}
