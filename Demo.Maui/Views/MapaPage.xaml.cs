using Demo.Maui.ViewModels;
using Demo.Shared.Dtos;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Demo.Maui.Views;

public partial class MapaPage : ContentPage
{
    private const string TituloSinCocheras = "No se encontraron cocheras";
    private const string MensajeSinCocheras =
        "No hay cocheras disponibles en este momento. Por favor, inténtelo de nuevo más tarde.";

    private readonly MapaViewModel _viewModel;

    public MapaPage(MapaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!await _viewModel.UsuarioPuedeUsarMapaComoClienteAsync())
        {
            await DisplayAlert(TituloSinCocheras, MensajeSinCocheras, "Aceptar");
            await Shell.Current.GoToAsync("//Inicio");
            return;
        }

        _viewModel.EstaCargando = true;
        map.Pins.Clear();
        List<CocheraDto> cocheras = await _viewModel.ObtenerCocherasAsync();

        foreach (CocheraDto cochera in cocheras)
        {
            Pin pinCochera = new()
            {
                Label = "Cochera disponible",
                Address = $"${cochera.Precio} por hora",
                Type = PinType.Place,
                Location = new Location(cochera.Latitud, cochera.Longitud),
                BindingContext = cochera
            };

            pinCochera.MarkerClicked += (sender, _) =>
            {
                if (sender is Pin pin && pin.BindingContext is CocheraDto cocheraSeleccionada)
                {
                    _viewModel.SeleccionarCochera(cocheraSeleccionada);
                }
            };

            map.Pins.Add(pinCochera);
        }

        _viewModel.EstaCargando = false;
        CentrarMapaEnRosario();
        await Task.Delay(200);
        MainThread.BeginInvokeOnMainThread(CentrarMapaEnRosario);
    }

    private void CentrarMapaEnRosario()
    {
        map.MoveToRegion(MapSpan.FromCenterAndRadius(_viewModel.CentroMapa, Distance.FromKilometers(0.6)));
    }
}
