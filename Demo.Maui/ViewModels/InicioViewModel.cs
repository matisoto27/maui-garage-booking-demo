using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo.Maui.Services;
using Microsoft.Maui.Controls;

namespace Demo.Maui.ViewModels;

public partial class InicioViewModel : ObservableObject
{
    private readonly UsuarioApiService _usuarioApiService;

    [ObservableProperty]
    private bool estaCargando = true;

    [ObservableProperty]
    private string saldo = "$0";

    [ObservableProperty]
    private bool tieneCocheras;

    public InicioViewModel(UsuarioApiService usuarioApiService)
    {
        _usuarioApiService = usuarioApiService;
    }

    [RelayCommand]
    private void MenuInicio()
    {
    }

    [RelayCommand]
    private void AliasCvu()
    {
    }

    [RelayCommand]
    private void CrearCochera()
    {
    }

    [RelayCommand]
    private void AdministrarCocheras()
    {
    }

    [RelayCommand]
    private async Task Mapa()
    {
        await Shell.Current.GoToAsync("//Mapa");
    }

    [RelayCommand]
    private void VerReservas()
    {
    }

    [RelayCommand]
    private void VerCalificaciones()
    {
    }

    public async Task InicializarAsync()
    {
        EstaCargando = true;
        Saldo = "$0";

        int cantidadCocheras = await _usuarioApiService.ObtenerCantidadCocherasAsync(SesionDemoContext.IdUsuario);
        TieneCocheras = cantidadCocheras > 0;

        EstaCargando = false;
    }
}
