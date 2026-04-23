using System.Text;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo.Maui.Services;
using Demo.Shared.Dtos;
using Microsoft.Maui.Controls;

namespace Demo.Maui.ViewModels;

public partial class IniciarSesionViewModel : ObservableObject
{
    private readonly UsuarioApiService _usuarioApiService;

    [ObservableProperty]
    private bool estaCargando;

    [ObservableProperty]
    private string? dni;

    [ObservableProperty]
    private string? contrasena;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    [ObservableProperty]
    private bool existeError;

    public IniciarSesionViewModel(UsuarioApiService usuarioApiService)
    {
        _usuarioApiService = usuarioApiService;
    }

    [RelayCommand]
    private async Task IniciarSesion()
    {
        EstaCargando = true;
        MensajeError = string.Empty;
        ExisteError = false;

        try
        {
            StringBuilder errores = new();

            if (string.IsNullOrWhiteSpace(Dni) || string.IsNullOrWhiteSpace(Contrasena))
            {
                MostrarError("(*) El DNI y la contraseña son obligatorios.");
                return;
            }

            if (!Regex.IsMatch(Dni, @"^\d{8}$"))
            {
                errores.AppendLine("(*) El DNI debe estar compuesto por 8 dígitos.");
            }

            if (Contrasena.Length < 8)
            {
                errores.AppendLine("(*) La contraseña debe tener al menos 8 caracteres.");
            }

            if (errores.Length > 0)
            {
                MostrarError(errores.ToString().Trim());
                return;
            }

            IniciarSesionResponseDto respuesta = await _usuarioApiService.IniciarSesionAsync(Dni!, Contrasena!);
            if (!respuesta.Exito)
            {
                MostrarError(respuesta.Mensaje);
                return;
            }

            SesionDemoContext.EstablecerUsuario(respuesta.IdUsuario);
            await Shell.Current.GoToAsync("//Inicio");
        }
        finally
        {
            await Task.Delay(300);
            EstaCargando = false;
        }
    }

    [RelayCommand]
    private void Registrarse()
    {
    }

    [RelayCommand]
    private void OlvidasteTuContrasena()
    {
    }

    private void MostrarError(string mensaje)
    {
        MensajeError = mensaje;
        ExisteError = true;
    }
}
