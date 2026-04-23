using Demo.Maui.ViewModels;

namespace Demo.Maui.Views;

public partial class IniciarSesionPage : ContentPage
{
    public IniciarSesionPage(IniciarSesionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is IniciarSesionViewModel viewModel)
        {
            viewModel.Dni = string.Empty;
            viewModel.Contrasena = string.Empty;
        }
    }
}
