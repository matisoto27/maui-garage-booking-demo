using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo.Maui.Services;
using Demo.Shared.Dtos;
using Microsoft.Maui.Controls;

namespace Demo.Maui.ViewModels;

public partial class InformacionDuenoViewModel : ObservableObject, IQueryAttributable
{
    private readonly UsuarioApiService _usuarioApiService;
    private int _idCochera;

    [ObservableProperty]
    private bool estaCargando = true;

    [ObservableProperty]
    private string dni = string.Empty;

    [ObservableProperty]
    private string apellidoNombre = string.Empty;

    [ObservableProperty]
    private string telefono = string.Empty;

    [ObservableProperty]
    private string direccionCompleta = string.Empty;

    [ObservableProperty]
    private string puntuacionDueno = string.Empty;

    [ObservableProperty]
    private string entreCalles = string.Empty;

    [ObservableProperty]
    private string precio = string.Empty;

    [ObservableProperty]
    private string puntuacionCochera = string.Empty;

    [ObservableProperty]
    private bool mostrarTextoSinCalificacionesDeOtrosDueno;

    [ObservableProperty]
    private bool mostrarVerMasDueno = true;

    [ObservableProperty]
    private bool mostrarTextoSinCalificacionesDeOtrosCochera;

    [ObservableProperty]
    private bool mostrarVerMasCochera = true;

    public InformacionDuenoViewModel(UsuarioApiService usuarioApiService)
    {
        _usuarioApiService = usuarioApiService;
    }

    [RelayCommand]
    private void VerMasDueno()
    {
    }

    [RelayCommand]
    private void VerMasCochera()
    {
    }

    [RelayCommand]
    private async Task Siguiente()
    {
        await Shell.Current.GoToAsync($"//SolicitarReserva?idCochera={_idCochera}");
    }

    [RelayCommand]
    private async Task Volver()
    {
        await Shell.Current.GoToAsync("//Mapa");
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("idCochera", out object? value) && int.TryParse(value?.ToString(), out int id))
        {
            _idCochera = id;
        }

        await InicializarAsync();
    }

    public async Task InicializarAsync()
    {
        if (_idCochera <= 0)
        {
            return;
        }

        EstaCargando = true;
        InformacionDuenoDto? info = await _usuarioApiService.ObtenerInformacionDuenoAsync(_idCochera);
        if (info is not null)
        {
            Dni = info.Dni;
            ApellidoNombre = info.ApellidoNombre;
            Telefono = info.Telefono;
            DireccionCompleta = info.DireccionCompleta;
            EntreCalles = info.EntreCalles;
            Precio = info.Precio;
            PuntuacionDueno = info.PuntuacionDueno;
            PuntuacionCochera = info.PuntuacionCochera;
            MostrarTextoSinCalificacionesDeOtrosDueno = info.MostrarTextoSinCalificacionesDeOtrosDueno;
            MostrarVerMasDueno = info.MostrarVerMasDueno;
            MostrarTextoSinCalificacionesDeOtrosCochera = info.MostrarTextoSinCalificacionesDeOtrosCochera;
            MostrarVerMasCochera = info.MostrarVerMasCochera;
        }

        EstaCargando = false;
    }
}
