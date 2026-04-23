using Demo.Maui.ViewModels;

namespace Demo.Maui.Views;

public partial class ReservaClientePage : ContentPage
{
    public ReservaClientePage(ReservaClienteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is ReservaClienteViewModel vm)
        {
            vm.DetenerTemporizador();
        }
    }
}
